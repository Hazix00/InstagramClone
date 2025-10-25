using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using InstagramClone.Client.Services;

namespace InstagramClone.Client.Pages;

public partial class Home : ComponentBase
{
    [Inject] public PostService PostService { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;

    private List<PostDto>? feed;
    private bool isLoading = true;
    private string? errorMessage;

    protected override async Task OnInitializedAsync()
    {
        // Check if user is authenticated before loading feed
        var authState = await AuthStateProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity?.IsAuthenticated == true)
        {
            await LoadFeed();
        }
        else
        {
            isLoading = false;
        }
    }

    private async Task LoadFeed()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            feed = await PostService.GetFeedAsync();
            
            if (feed == null)
            {
                feed = new List<PostDto>();
            }
        }
        catch (Exception ex)
        {
            // 401 errors are now handled globally by AuthenticationHandler
            // which will redirect to login automatically
            errorMessage = $"Failed to load feed: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task ReloadFeed()
    {
        await LoadFeed();
        StateHasChanged();
    }
}

