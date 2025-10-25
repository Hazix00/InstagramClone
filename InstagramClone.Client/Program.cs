using Blazored.LocalStorage;
using InstagramClone.Client;
using InstagramClone.Client.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.JSInterop;
using System.Globalization;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

// Register the authentication handler
builder.Services.AddScoped<AuthenticationHandler>();

// Configure HttpClient with Gateway base address and authentication handler
var gatewayUrl = builder.Configuration["ApiGateway:BaseUrl"] 
                 ?? throw new InvalidOperationException("ApiGateway:BaseUrl is not configured");

builder.Services.AddHttpClient("API", client => 
{
    client.BaseAddress = new Uri(gatewayUrl);
})
.AddHttpMessageHandler<AuthenticationHandler>();

// Register the default HttpClient (uses the named "API" client)
builder.Services.AddScoped(sp => 
    sp.GetRequiredService<IHttpClientFactory>().CreateClient("API"));

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PostService>();
builder.Services.AddScoped<CommentService>();
builder.Services.AddScoped<FollowService>();

var host = builder.Build();

// Load culture from localStorage (via JS) and set it
var js = host.Services.GetRequiredService<IJSRuntime>();
string? cultureName = null;
try
{
    cultureName = await js.InvokeAsync<string?>("blazorCulture.get");
}
catch
{
    // If JS interop isn't available yet, fall back to default
}
var culture = new CultureInfo(string.IsNullOrWhiteSpace(cultureName) ? "en" : cultureName);
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;

await host.RunAsync();
