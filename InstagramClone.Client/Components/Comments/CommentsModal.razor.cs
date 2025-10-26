using InstagramClone.Client.Services;
using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Components;

namespace InstagramClone.Client.Components.Comments;

public partial class CommentsModal : ComponentBase
{
    [Parameter] public bool IsOpen { get; set; }
    [Parameter] public PostDto? Post { get; set; }
    [Parameter] public EventCallback OnClose { get; set; }
    [Parameter] public EventCallback OnPostLikeToggle { get; set; }
    
    [Inject] private TimeService TimeService { get; set; } = default!;

    private List<CommentDto> comments = new();
    private Dictionary<Guid, List<CommentDto>> repliesByComment = new();
    private HashSet<Guid> expandedReplies = new();
    private bool isLoadingComments = false;
    private CommentDto? replyingTo;

    protected override async Task OnParametersSetAsync()
    {
        if (IsOpen && Post != null && comments.Count == 0)
        {
            await LoadComments();
        }
        else if (!IsOpen)
        {
            // Reset state when closed
            comments.Clear();
            repliesByComment.Clear();
            expandedReplies.Clear();
            replyingTo = null;
        }
    }

    private async Task LoadComments()
    {
        if (Post == null) return;

        isLoadingComments = true;
        try
        {
            var result = await CommentService.GetCommentsForPostAsync(Post.Id);
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
        if (expandedReplies.Contains(commentId))
        {
            expandedReplies.Remove(commentId);
            return;
        }

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
            StateHasChanged();
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
    }

    private void CancelReply()
    {
        replyingTo = null;
    }

    private async Task AddComment(string content)
    {
        if (string.IsNullOrWhiteSpace(content) || Post == null) return;

        try
        {
            var newComment = await CommentService.AddCommentAsync(
                Post.Id,
                content,
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
                    if (Post != null)
                    {
                        Post = Post with
                        {
                            CommentsCount = Post.CommentsCount + 1
                        };
                    }
                }

                replyingTo = null;
                StateHasChanged();
            }
        }
        catch
        {
            // Handle error
        }
    }

    private async Task Close()
    {
        await OnClose.InvokeAsync();
    }

    private string GetTimeAgo(DateTime dateTime) => TimeService.GetInstagramTimeAgo(dateTime);
}

