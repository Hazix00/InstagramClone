using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace InstagramClone.Client.Pages.Profile;

public partial class Profile : ComponentBase
{
    [Inject] public HttpClient Http { get; set; } = default!;
    [Inject] public InstagramClone.Client.Services.PostService PostService { get; set; } = default!;
    [Inject] public NavigationManager Navigation { get; set; } = default!;

    private UserProfileDto? UserProfile { get; set; }
    private string? ErrorMessage { get; set; }
    private bool IsLoading { get; set; } = true;
    private List<PostDto> MyPosts { get; set; } = new();

    protected override async Task OnInitializedAsync()
    {
        await LoadProfile();
        MyPosts = await PostService.GetMyPostsAsync() ?? new();
    }

    private async Task LoadProfile()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            UserProfile = await Http.GetFromJsonAsync<UserProfileDto>("api/auth/profile");
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
}

