using InstagramClone.Core.DTOs;
using InstagramClone.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InstagramClone.Client.Pages.Login;

public partial class Login : ComponentBase
{
    [Inject] public AuthService AuthService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private LoginRequest LoginRequestModel { get; set; } = new();
    private string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; }

    private async Task HandleLogin()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            var result = await AuthService.LoginAsync(LoginRequestModel);

            if (result != null)
            {
                // Notify the auth state provider
                if (AuthStateProvider is CustomAuthStateProvider customAuthStateProvider)
                {
                    customAuthStateProvider.NotifyUserAuthentication(result.Token);
                }

                Navigation.NavigateTo("/");
            }
            else
            {
                ErrorMessage = "Sorry, your username or password was incorrect. Please double-check your credentials.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }
}

