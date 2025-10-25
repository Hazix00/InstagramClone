using InstagramClone.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InstagramClone.Client.Layout;

public partial class MainLayout : LayoutComponentBase
{
    [Inject] public AuthService AuthService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private async Task HandleLogout()
    {
        // Clear token from localStorage
        await AuthService.LogoutAsync();
        
        // Notify authentication state provider
        if (AuthStateProvider is CustomAuthStateProvider customProvider)
        {
            customProvider.NotifyUserLogout();
        }
        
        // Redirect to home page (will show welcome screen for unauthenticated users)
        Navigation.NavigateTo("/", forceLoad: true);
    }
}

