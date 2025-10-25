namespace InstagramClone.Gateway.Middleware;

public class GatewayKeyMiddleware(RequestDelegate next, IConfiguration configuration)
{
    private readonly RequestDelegate _next = next;
    private readonly IConfiguration _configuration = configuration;
    private const string GatewayKeyHeader = "X-Gateway-Key";

    public async Task InvokeAsync(HttpContext context)
    {
        var apiKey = _configuration["Gateway:ApiKey"];
        
        if (!string.IsNullOrEmpty(apiKey))
        {
            // Add Gateway key to all outgoing requests
            context.Request.Headers.Append(GatewayKeyHeader, apiKey);
        }

        await _next(context);
    }
}

public static class GatewayKeyMiddlewareExtensions
{
    public static IApplicationBuilder UseGatewayKey(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GatewayKeyMiddleware>();
    }
}

