using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using InstagramClone.Client.Services;
using Microsoft.JSInterop;

namespace InstagramClone.Client.Pages;

public partial class Home : ComponentBase, IAsyncDisposable
{
    [Inject] public PostService PostService { get; set; } = default!;
    [Inject] public AuthenticationStateProvider AuthStateProvider { get; set; } = default!;
    [Inject] public IJSRuntime JS { get; set; } = default!;

    private List<PostDto> feed = new();
    private bool isLoading = true;
    private bool isLoadingMore = false;
    private string? errorMessage;
    private int skip = 0;
    private const int take = 5;
    private bool hasMore = true;
    private DotNetObjectReference<Home>? dotNetRef;
    private IJSObjectReference? scrollModule;

    // Comments Modal State
    private bool isCommentsModalOpen = false;
    private PostDto? selectedPost;

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

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender && feed.Any())
        {
            try
            {
                dotNetRef = DotNetObjectReference.Create(this);
                scrollModule = await JS.InvokeAsync<IJSObjectReference>("import", "./js/infinite-scroll.js");
                await scrollModule.InvokeVoidAsync("initInfiniteScroll", dotNetRef);
            }
            catch
            {
                // JS interop might not be available yet
            }
        }
    }

    private async Task LoadFeed()
    {
        isLoading = true;
        errorMessage = null;

        try
        {
            var posts = await PostService.GetFeedAsync(take, 0);
            
            if (posts != null && posts.Any())
            {
                feed = posts;
                skip = feed.Count;
                hasMore = posts.Count >= take;
            }
            else
            {
                feed = new List<PostDto>();
                hasMore = false;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to load feed: {ex.Message}";
        }
        finally
        {
            isLoading = false;
        }
    }

    [JSInvokable]
    public async Task LoadMore()
    {
        if (isLoadingMore || !hasMore) return;

        isLoadingMore = true;
        StateHasChanged();

        try
        {
            var posts = await PostService.GetFeedAsync(take, skip);
            
            if (posts != null && posts.Any())
            {
                feed.AddRange(posts);
                skip += posts.Count;
                hasMore = posts.Count >= take;
            }
            else
            {
                hasMore = false;
            }
        }
        catch (Exception ex)
        {
            errorMessage = $"Failed to load more posts: {ex.Message}";
        }
        finally
        {
            isLoadingMore = false;
            StateHasChanged();
        }
    }

    private async Task ToggleLike(PostDto post)
    {
        try
        {
            bool success;
            if (post.IsLikedByCurrentUser)
            {
                success = await PostService.UnlikeAsync(post.Id);
            }
            else
            {
                success = await PostService.LikeAsync(post.Id);
            }

            if (success)
            {
                // Update the post in the feed
                var index = feed.FindIndex(p => p.Id == post.Id);
                if (index >= 0)
                {
                    feed[index] = post with 
                    { 
                        IsLikedByCurrentUser = !post.IsLikedByCurrentUser,
                        LikesCount = post.IsLikedByCurrentUser ? post.LikesCount - 1 : post.LikesCount + 1
                    };

                    // Update selected post if modal is open
                    if (selectedPost?.Id == post.Id)
                    {
                        selectedPost = feed[index];
                    }

                    StateHasChanged();
                }
            }
        }
        catch
        {
            // Handle error silently
        }
    }

    private void OpenComments(PostDto post)
    {
        selectedPost = post;
        isCommentsModalOpen = true;
        StateHasChanged();
    }

    private void CloseComments()
    {
        isCommentsModalOpen = false;
        selectedPost = null;
        StateHasChanged();
    }

    public async ValueTask DisposeAsync()
    {
        if (scrollModule != null)
        {
            try
            {
                await scrollModule.InvokeVoidAsync("cleanup");
                await scrollModule.DisposeAsync();
            }
            catch
            {
                // Cleanup error
            }
        }
        
        dotNetRef?.Dispose();
    }
}
