namespace InstagramClone.Api.Middleware;

public class GatewayAuthenticationMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;
    private const string GatewayKeyHeader = "X-Gateway-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        // Allow health check endpoint without gateway key
        if (context.Request.Path.StartsWithSegments("/health"))
        {
            await _next(context);
            return;
        }

        var expectedKey = _configuration["Gateway:ApiKey"];
        
        if (string.IsNullOrEmpty(expectedKey))
        {
            // In development, if no key is configured, allow all requests
            if (_configuration.GetValue<string>("ASPNETCORE_ENVIRONMENT") == "Development")
            {
                await _next(context);
                return;
            }
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new { error = "Gateway key not configured" });
            return;
        }

        if (!context.Request.Headers.TryGetValue(GatewayKeyHeader, out var providedKey) 
            || providedKey != expectedKey)
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            await context.Response.WriteAsJsonAsync(new 
            { 
                error = "Direct access forbidden. Please use the API gateway.",
                gateway = "http://localhost:5200"
            });
            return;
        }

        await _next(context);
    }
}

public static class GatewayAuthenticationMiddlewareExtensions
{
    public static IApplicationBuilder UseGatewayAuthentication(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GatewayAuthenticationMiddleware>();
    }
}

