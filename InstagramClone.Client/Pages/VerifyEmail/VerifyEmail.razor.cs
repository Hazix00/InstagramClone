using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace InstagramClone.Client.Pages.VerifyEmail;

public partial class VerifyEmail : ComponentBase
{
    [Inject] public HttpClient Http { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    private VerifyEmailModel VerifyModel { get; set; } = new();
    private string? ErrorMessage { get; set; }
    private string? SuccessMessage { get; set; }
    private bool IsLoading { get; set; }
    private bool IsResending { get; set; }

    private readonly string authEndpoint = "api/auth";

    private async Task HandleVerifyEmail()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        IsLoading = true;

        try
        {
            var response = await Http.PostAsJsonAsync($"{authEndpoint}/verify-email", VerifyModel);

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Email verified successfully! You can now log in.";
            }
            else
            {
                ErrorMessage = "Invalid or expired verification code. Please try again or request a new code.";
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

    private async Task ResendVerification()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        IsResending = true;

        try
        {
            var response = await Http.PostAsJsonAsync($"{authEndpoint}/send-verification-email", new { email = VerifyModel.Email });

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "Verification email sent! Please check your inbox.";
            }
            else
            {
                ErrorMessage = "Failed to send verification email. Please try again later.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsResending = false;
        }
    }
}

public class VerifyEmailModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Verification code is required")]
    public string Token { get; set; } = string.Empty;
}

