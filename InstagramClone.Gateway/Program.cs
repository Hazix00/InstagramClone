using Yarp.ReverseProxy.Transforms;

var builder = WebApplication.CreateBuilder(args);

// Enable logging
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Get Gateway API Key from config
var gatewayApiKey = builder.Configuration["Gateway:ApiKey"] 
    ?? throw new InvalidOperationException("Gateway:ApiKey is not configured");

// Add YARP reverse proxy with request transformations
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"))
    .AddTransforms(builderContext =>
    {
        // Add X-Gateway-Key header to ALL proxied requests
        builderContext.AddRequestTransform(transformContext =>
        {
            transformContext.ProxyRequest.Headers.Add("X-Gateway-Key", gatewayApiKey);
            return ValueTask.CompletedTask;
        });
        
        // Note: YARP automatically forwards most headers including Authorization
        // We only need to add custom headers like X-Gateway-Key
    });

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
              .AllowCredentials(); // Important for SignalR and auth
    });
});

var app = builder.Build();

// Log all incoming requests for debugging
app.Use(async (context, next) =>
{
    var hasAuth = context.Request.Headers.ContainsKey("Authorization");
    app.Logger.LogInformation("Gateway received: {Method} {Path} from {RemoteIp}, HasAuth: {HasAuth}", 
        context.Request.Method, 
        context.Request.Path, 
        context.Connection.RemoteIpAddress,
        hasAuth);
    await next();
});

// Use CORS
app.UseCors();

// Map YARP routes (with transforms applied)
app.MapReverseProxy();

// Health check endpoint
app.MapGet("/health", () => Results.Ok(new { status = "healthy", service = "gateway" }));

app.Run();
