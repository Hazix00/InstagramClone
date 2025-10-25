using InstagramClone.Gateway.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Add YARP reverse proxy
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

// Add CORS
var allowedOrigins = builder.Configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() 
                     ?? Array.Empty<string>();

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials(); // Important for SignalR
    });
});

var app = builder.Build();

// Use CORS
app.UseCors();

// Add Gateway key to all proxied requests
app.UseGatewayKey();

// Map YARP routes
app.MapReverseProxy();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "gateway" }));

app.Run();
