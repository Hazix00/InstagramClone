using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net;
using System.Net.Http.Headers;

namespace InstagramClone.Client.Services;

/// <summary>
/// HTTP message handler that intercepts all HTTP requests to:
/// 1. Add JWT token to Authorization header
/// 2. Handle 401 Unauthorized responses globally
/// </summary>
public class AuthenticationHandler : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider;
    private const string TokenKey = "authToken";

    public AuthenticationHandler(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        // Resolve services on-demand to avoid circular dependency
        var localStorage = _serviceProvider.GetRequiredService<ILocalStorageService>();
        
        // Get token from localStorage
        var token = await localStorage.GetItemAsStringAsync(TokenKey);

        if (!string.IsNullOrEmpty(token))
        {
            token = token.Trim('"');
            
            // Add Authorization header if not already present
            if (request.Headers.Authorization == null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }
        }

        // Send the request
        var response = await base.SendAsync(request, cancellationToken);

        // Handle 401 Unauthorized globally
        if (response.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Resolve services for logout handling
            var navigationManager = _serviceProvider.GetRequiredService<NavigationManager>();
            var authStateProvider = _serviceProvider.GetRequiredService<AuthenticationStateProvider>();
            
            // Token is invalid or expired
            await localStorage.RemoveItemAsync(TokenKey);
            
            // Notify auth state provider of logout
            if (authStateProvider is CustomAuthStateProvider customProvider)
            {
                customProvider.NotifyUserLogout();
            }

            // Redirect to login
            navigationManager.NavigateTo("/login", forceLoad: true);
        }

        return response;
    }
}

