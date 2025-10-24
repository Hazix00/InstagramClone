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

// Configure HttpClient with API base address
builder.Services.AddScoped(sp => new HttpClient 
{ 
    BaseAddress = new Uri("http://localhost:5203") // API HTTP URL from launchSettings.json
});

// Add Blazored LocalStorage
builder.Services.AddBlazoredLocalStorage();

// Add Authentication services
builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthStateProvider>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<PostService>();
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
