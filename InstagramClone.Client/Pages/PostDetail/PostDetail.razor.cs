using InstagramClone.Client.Services;
using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;

namespace InstagramClone.Client.Pages.PostDetail;

public partial class PostDetail : ComponentBase
{
    [Parameter] public Guid PostId { get; set; }
    
    [Inject] public PostService PostService { get; set; } = default!;
    [Inject] public CommentService CommentService { get; set; } = default!;

    private PostDto? post;
    private List<CommentDto> comments = new();
    private Dictionary<Guid, List<CommentDto>> repliesByComment = new();
    private HashSet<Guid> expandedReplies = new();
    private Dictionary<Guid, bool> isLoadingReplies = new();
    
    private bool isLoading = true;
    private bool isLoadingComments = false;
    private string commentInput = string.Empty;
    private CommentDto? replyingTo;

    protected override async Task OnInitializedAsync()
    {
        await LoadPost();
        if (post != null)
        {
            await LoadComments();
        }
    }

    private async Task LoadPost()
    {
        isLoading = true;
        try
        {
            post = await PostService.GetByIdAsync(PostId);
        }
        catch
        {
            // Handle error
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task LoadComments()
    {
        isLoadingComments = true;
        try
        {
            var result = await CommentService.GetCommentsForPostAsync(PostId);
            if (result != null)
            {
                comments = result;
            }
        }
        catch
        {
            // Handle error
        }
        finally
        {
            isLoadingComments = false;
        }
    }

    private async Task LoadReplies(Guid commentId)
    {
        isLoadingReplies[commentId] = true;
        expandedReplies.Add(commentId);
        StateHasChanged();

        try
        {
            var replies = await CommentService.GetRepliesAsync(commentId);
            if (replies != null)
            {
                repliesByComment[commentId] = replies;
            }
        }
        catch
        {
            // Handle error
        }
        finally
        {
            isLoadingReplies[commentId] = false;
            StateHasChanged();
        }
    }

    private async Task TogglePostLike()
    {
        if (post == null) return;

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
                post = post with
                {
                    IsLikedByCurrentUser = !post.IsLikedByCurrentUser,
                    LikesCount = post.IsLikedByCurrentUser ? post.LikesCount - 1 : post.LikesCount + 1
                };
                StateHasChanged();
            }
        }
        catch
        {
            // Handle error silently
        }
    }

    private async Task ToggleCommentLike(CommentDto comment)
    {
        try
        {
            bool success;
            if (comment.IsLikedByCurrentUser)
            {
                success = await CommentService.UnlikeAsync(comment.Id);
            }
            else
            {
                success = await CommentService.LikeAsync(comment.Id);
            }

            if (success)
            {
                // Update in comments list
                var index = comments.FindIndex(c => c.Id == comment.Id);
                if (index >= 0)
                {
                    comments[index] = comment with
                    {
                        IsLikedByCurrentUser = !comment.IsLikedByCurrentUser,
                        LikesCount = comment.IsLikedByCurrentUser ? comment.LikesCount - 1 : comment.LikesCount + 1
                    };
                }
                
                // Update in replies
                foreach (var kvp in repliesByComment.ToList())
                {
                    var replyIndex = kvp.Value.FindIndex(r => r.Id == comment.Id);
                    if (replyIndex >= 0)
                    {
                        var updatedReplies = kvp.Value.ToList();
                        updatedReplies[replyIndex] = comment with
                        {
                            IsLikedByCurrentUser = !comment.IsLikedByCurrentUser,
                            LikesCount = comment.IsLikedByCurrentUser ? comment.LikesCount - 1 : comment.LikesCount + 1
                        };
                        repliesByComment[kvp.Key] = updatedReplies;
                    }
                }
                
                StateHasChanged();
            }
        }
        catch
        {
            // Handle error silently
        }
    }

    private void StartReply(CommentDto comment)
    {
        replyingTo = comment;
        commentInput = string.Empty;
    }

    private void CancelReply()
    {
        replyingTo = null;
        commentInput = string.Empty;
    }

    private async Task AddComment()
    {
        if (string.IsNullOrWhiteSpace(commentInput) || post == null) return;

        try
        {
            var newComment = await CommentService.AddCommentAsync(
                post.Id,
                commentInput,
                replyingTo?.Id
            );

            if (newComment != null)
            {
                if (replyingTo != null)
                {
                    // Add to replies
                    if (!repliesByComment.ContainsKey(replyingTo.Id))
                    {
                        repliesByComment[replyingTo.Id] = new List<CommentDto>();
                    }
                    repliesByComment[replyingTo.Id].Add(newComment);
                    
                    // Update reply count
                    var commentIndex = comments.FindIndex(c => c.Id == replyingTo.Id);
                    if (commentIndex >= 0)
                    {
                        comments[commentIndex] = comments[commentIndex] with
                        {
                            RepliesCount = comments[commentIndex].RepliesCount + 1
                        };
                    }
                    
                    expandedReplies.Add(replyingTo.Id);
                }
                else
                {
                    // Add to comments
                    comments.Insert(0, newComment);
                    
                    // Update post comment count
                    post = post with
                    {
                        CommentsCount = post.CommentsCount + 1
                    };
                }

                commentInput = string.Empty;
                replyingTo = null;
                StateHasChanged();
            }
        }
        catch
        {
            // Handle error
        }
    }
}

