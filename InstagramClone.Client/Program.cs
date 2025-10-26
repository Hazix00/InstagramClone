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

// Add Localization
builder.Services.AddLocalization();

// Add utility services
builder.Services.AddScoped<TimeService>();

var host = builder.Build();

// Load culture from localStorage (via JS) and set it
var js = host.Services.GetRequiredService<IJSRuntime>();
string? cultureName = null;
try
{
    cultureName = await js.InvokeAsync<string?>("blazorCulture.get");
    Console.WriteLine($"[Program.cs] Loaded culture from localStorage: {cultureName ?? "null"}");
}
catch (Exception ex)
{
    Console.WriteLine($"[Program.cs] Failed to load culture: {ex.Message}");
    // If JS interop isn't available yet, fall back to default
}
var culture = new CultureInfo(string.IsNullOrWhiteSpace(cultureName) ? "en" : cultureName);
Console.WriteLine($"[Program.cs] Setting DefaultThreadCurrentCulture to: {culture.Name}");
Console.WriteLine($"[Program.cs] Setting DefaultThreadCurrentUICulture to: {culture.Name}");
CultureInfo.DefaultThreadCurrentCulture = culture;
CultureInfo.DefaultThreadCurrentUICulture = culture;
Console.WriteLine($"[Program.cs] Current culture after set: {CultureInfo.CurrentCulture.Name}");
Console.WriteLine($"[Program.cs] Current UI culture after set: {CultureInfo.CurrentUICulture.Name}");

await host.RunAsync();
