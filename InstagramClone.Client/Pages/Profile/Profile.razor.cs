using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using System.Security.Claims;
using InstagramClone.Client.Services;

namespace InstagramClone.Client.Pages.Profile;

public partial class Profile : ComponentBase
{
    [Inject] public HttpClient Http { get; set; } = default!;
    [Inject] public PostService PostService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    
    [Parameter] public string? Username { get; set; }

    private UserProfileDto? UserProfile { get; set; }
    private string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; } = true;
    private List<PostDto> MyPosts { get; set; } = new();
    private bool IsOwnProfile { get; set; }
    private string? CurrentUsername { get; set; }

    protected override async Task OnInitializedAsync()
    {
        await LoadCurrentUser();
        await LoadProfile();
        await LoadPosts();
    }

    protected override async Task OnParametersSetAsync()
    {
        // Reload when username parameter changes
        if (UserProfile != null && Username != UserProfile.Username)
        {
            await LoadProfile();
            await LoadPosts();
        }
    }

    private async Task LoadCurrentUser()
    {
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;
        
        if (user.Identity?.IsAuthenticated == true)
        {
            CurrentUsername = user.FindFirst(ClaimTypes.Name)?.Value 
                           ?? user.FindFirst("preferred_username")?.Value 
                           ?? user.FindFirst("unique_name")?.Value;
        }
    }

    private async Task LoadProfile()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            // If username parameter is provided, load that user's profile
            // Otherwise load current user's profile
            var targetUsername = Username ?? CurrentUsername;
            
            if (string.IsNullOrEmpty(targetUsername))
            {
                UserProfile = await Http.GetFromJsonAsync<UserProfileDto>("api/auth/profile");
                IsOwnProfile = true;
            }
            else
            {
                UserProfile = await Http.GetFromJsonAsync<UserProfileDto>($"api/auth/profile/{targetUsername}");
                IsOwnProfile = string.Equals(targetUsername, CurrentUsername, StringComparison.OrdinalIgnoreCase);
            }
        }
        catch (HttpRequestException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            ErrorMessage = "User not found";
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

    private async Task LoadPosts()
    {
        try
        {
            // For now, only load own posts
            // You can extend this to load any user's posts later
            if (IsOwnProfile)
            {
                MyPosts = await PostService.GetMyPostsAsync() ?? new();
            }
            else
            {
                // TODO: Add API endpoint to get posts by username
                MyPosts = new();
            }
        }
        catch
        {
            MyPosts = new();
        }
    }
}

