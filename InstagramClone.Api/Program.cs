using System.Text;
using InstagramClone.Api.Data;
using InstagramClone.Api.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Globalization;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddLocalization();
builder.Services.AddControllers();

// Configure Entity Framework with PostgreSQL
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Register JWT Token Service
builder.Services.AddScoped<JwtTokenService>();

// Register Repositories
builder.Services.AddScoped(typeof(InstagramClone.Api.Repositories.IRepository<>), typeof(InstagramClone.Api.Repositories.Repository<>));
builder.Services.AddScoped<InstagramClone.Api.Repositories.IUserRepository, InstagramClone.Api.Repositories.UserRepository>();
builder.Services.AddScoped<InstagramClone.Api.Repositories.IPostRepository, InstagramClone.Api.Repositories.PostRepository>();
builder.Services.AddScoped<InstagramClone.Api.Repositories.ICommentRepository, InstagramClone.Api.Repositories.CommentRepository>();
builder.Services.AddScoped<InstagramClone.Api.Repositories.IFollowRepository, InstagramClone.Api.Repositories.FollowRepository>();

// Register Services
builder.Services.AddScoped(typeof(InstagramClone.Api.Services.IService<>), typeof(InstagramClone.Api.Services.Service<>));
builder.Services.AddScoped<InstagramClone.Api.Services.IPostService, InstagramClone.Api.Services.PostService>();
builder.Services.AddScoped<InstagramClone.Api.Services.ICommentService, InstagramClone.Api.Services.CommentService>();
builder.Services.AddScoped<InstagramClone.Api.Services.IFollowService, InstagramClone.Api.Services.FollowService>();

// Configure JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key not configured");
var key = Encoding.ASCII.GetBytes(jwtKey);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
});

// Add CORS policy for Blazor client
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowBlazorClient", policy =>
    {
        policy.WithOrigins(
                "https://localhost:7111", // Client HTTPS from launchSettings
                "http://localhost:5245"   // Client HTTP from launchSettings
            )
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });
});

// Add Swagger/OpenAPI support
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Instagram Clone API",
        Version = "v1",
        Description = "API for Instagram Clone application"
    });

    // Add JWT Authentication to Swagger
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme. Enter 'Bearer' [space] and then your token in the text input below.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

var app = builder.Build();

// Localization (en, fr, ar)
var supportedCultures = new[] { "en", "fr", "ar" };
var localizationOptions = new RequestLocalizationOptions()
    .SetDefaultCulture("en")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures);

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "Instagram Clone API v1");
        options.RoutePrefix = string.Empty; // Set Swagger UI at the app's root
    });
}

app.UseHttpsRedirection();

app.UseRequestLocalization(localizationOptions);

app.UseCors("AllowBlazorClient");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
