using InstagramClone.Core.DTOs;
using InstagramClone.Client.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;

namespace InstagramClone.Client.Pages.Register;

public partial class Register : ComponentBase
{
    [Inject] public AuthService AuthService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private RegisterRequest RegisterRequestModel { get; set; } = new();
    private string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; }

    private async Task HandleRegister()
    {
        ErrorMessage = null;
        IsLoading = true;

        try
        {
            var result = await AuthService.RegisterAsync(RegisterRequestModel);

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
                ErrorMessage = "Registration failed. Username or email may already exist.";
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

