using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using Microsoft.JSInterop;

namespace InstagramClone.Client.Pages.Settings;

public partial class Settings : ComponentBase
{
    [Inject] public HttpClient Http { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    private UserProfileDto? CurrentProfile { get; set; }
    private UpdateProfileRequest UpdateRequestModel { get; set; } = new();
    private string? ErrorMessage { get; set; }
    private string? SuccessMessage { get; set; }
    private bool IsLoading { get; set; } = true;
    private bool IsSaving { get; set; }
    private string CurrentLanguage { get; set; } = "en";
    private string CurrentTheme { get; set; } = "system";

    private readonly string authEndpoint = "api/auth";

    protected override async Task OnInitializedAsync()
    {
        await LoadProfile();
        await LoadLanguage();
        await LoadTheme();
    }

    private async Task LoadProfile()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            CurrentProfile = await Http.GetFromJsonAsync<UserProfileDto>($"{authEndpoint}/profile");
            
            if (CurrentProfile != null)
            {
                UpdateRequestModel.Username = CurrentProfile.Username;
                UpdateRequestModel.Email = CurrentProfile.Email;
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Failed to load profile: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task LoadLanguage()
    {
        try
        {
            var lang = await JS.InvokeAsync<string?>("blazorCulture.get");
            CurrentLanguage = string.IsNullOrWhiteSpace(lang) ? "en" : lang;
        }
        catch
        {
            CurrentLanguage = "en";
        }
    }

    private async Task HandleUpdateProfile()
    {
        ErrorMessage = null;
        SuccessMessage = null;
        IsSaving = true;

        try
        {
            var response = await Http.PutAsJsonAsync($"{authEndpoint}/profile", UpdateRequestModel);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<dynamic>();
                SuccessMessage = "Profile updated successfully!";
                
                // Reload profile to get updated data
                await LoadProfile();

                // If username changed, we might need to update the auth state
                // For now, we'll just show a success message
            }
            else
            {
                var errorContent = await response.Content.ReadAsStringAsync();
                ErrorMessage = "Failed to update profile. Please try again.";
            }
        }
        catch (Exception ex)
        {
            ErrorMessage = $"An error occurred: {ex.Message}";
        }
        finally
        {
            IsSaving = false;
        }
    }

    private async Task LoadTheme()
    {
        try
        {
            var theme = await JS.InvokeAsync<string?>("themeManager.get");
            CurrentTheme = string.IsNullOrWhiteSpace(theme) ? "system" : theme;
        }
        catch
        {
            CurrentTheme = "system";
        }
    }

    private async Task OnLanguageChanged(ChangeEventArgs e)
    {
        var selected = e.Value?.ToString() ?? "en";
        await JS.InvokeVoidAsync("blazorCulture.set", selected);
        await JS.InvokeVoidAsync("blazorCulture.apply", selected);
        // Reload to apply thread cultures and re-render localized strings
        Navigation.NavigateTo(Navigation.Uri, forceLoad: true);
    }

    private async Task OnThemeChanged(ChangeEventArgs e)
    {
        var selected = e.Value?.ToString() ?? "system";
        CurrentTheme = selected;
        
        try
        {
            if (selected == "system")
            {
                // Remove saved preference, use system default
                await JS.InvokeVoidAsync("localStorage.removeItem", "theme");
                // Apply system preference
                var isDark = await JS.InvokeAsync<bool>("eval", "window.matchMedia('(prefers-color-scheme: dark)').matches");
                await JS.InvokeVoidAsync("themeManager.apply", isDark ? "dark" : "light");
            }
            else
            {
                // Save and apply user preference
                await JS.InvokeVoidAsync("themeManager.set", selected);
            }
        }
        catch (Exception ex)
        {
            // Fallback if JS interop fails
            Console.WriteLine($"Theme change error: {ex.Message}");
        }
    }
}

