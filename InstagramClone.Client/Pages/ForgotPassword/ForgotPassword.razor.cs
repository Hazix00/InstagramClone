using Microsoft.AspNetCore.Components;
using System.ComponentModel.DataAnnotations;
using System.Net.Http.Json;

namespace InstagramClone.Client.Pages.ForgotPassword;

public partial class ForgotPassword : ComponentBase
{
    [Inject] public HttpClient Http { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    private ForgotPasswordEmailModel EmailModel { get; set; } = new();
    private string? ErrorMessage { get; set; }
    private string? SuccessMessage { get; set; }
    private bool IsLoading { get; set; }

    private readonly string authEndpoint = "api/auth";

    private async Task HandleForgotPassword()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        IsLoading = true;

        try
        {
            var response = await Http.PostAsJsonAsync($"{authEndpoint}/forgot-password", new { email = EmailModel.Email });

            if (response.IsSuccessStatusCode)
            {
                SuccessMessage = "If an account exists with this email, you will receive a password reset link shortly.";
                EmailModel = new ForgotPasswordEmailModel(); // Clear form
            }
            else
            {
                ErrorMessage = "An error occurred. Please try again later.";
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

public class ForgotPasswordEmailModel
{
    [Required(ErrorMessage = "Email is required")]
    [EmailAddress(ErrorMessage = "Invalid email address")]
    public string Email { get; set; } = string.Empty;
}

