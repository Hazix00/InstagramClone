using InstagramClone.Core.Entities;
using InstagramClone.Api.Repositories;

namespace InstagramClone.Api.Services;

public class CommentService(ICommentRepository commentRepository, IPostRepository postRepository) : Service<Comment>(commentRepository), ICommentService
{
    private readonly ICommentRepository _commentRepository = commentRepository;
    private readonly IPostRepository _postRepository = postRepository;

    public async Task<(int total, List<Comment> items)> GetTopLevelCommentsAsync(Guid postId, int take, int skip)
    {
        return await _commentRepository.GetTopLevelCommentsAsync(postId, take, skip);
    }

    public async Task<(int total, List<Comment> items)> GetRepliesAsync(Guid parentId, int take, int skip)
    {
        return await _commentRepository.GetRepliesAsync(parentId, take, skip);
    }

    public async Task<int> GetReplyCountAsync(Guid commentId)
    {
        return await _commentRepository.GetReplyCountAsync(commentId);
    }

    public async Task<Comment?> CreateCommentAsync(Guid postId, int userId, string content, Guid? parentCommentId)
    {
        // Validate post exists
        if (!await _postRepository.ExistsAsync(postId))
            return null;

        // Validate parent comment if provided
        if (parentCommentId.HasValue)
        {
            var parent = await _commentRepository.GetByIdAsync(parentCommentId.Value);
            if (parent == null || parent.PostId != postId)
                return null;

            // Enforce one-level replies only (Instagram style)
            if (parent.ParentCommentId != null)
                return null;
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = userId,
            Content = content,
            ParentCommentId = parentCommentId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddAsync(comment);
        await _commentRepository.SaveChangesAsync();

        return await _commentRepository.GetByIdAsync(comment.Id);
    }

    public async Task<bool> DeleteCommentAsync(Guid commentId, int userId)
    {
        var comment = await _commentRepository.GetByIdAsync(commentId);
        if (comment == null || comment.UserId != userId)
            return false;

        await _commentRepository.DeleteAsync(comment);
        await _commentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> LikeCommentAsync(Guid commentId, int userId)
    {
        if (!await _commentRepository.ExistsAsync(commentId))
            return false;

        if (await _commentRepository.IsLikedByUserAsync(commentId, userId))
            return true;

        var like = new CommentLike
        {
            CommentId = commentId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _commentRepository.AddLikeAsync(like);
        await _commentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlikeCommentAsync(Guid commentId, int userId)
    {
        var like = await _commentRepository.GetLikeAsync(commentId, userId);
        if (like == null)
            return true;

        await _commentRepository.RemoveLikeAsync(like);
        await _commentRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsLikedByUserAsync(Guid commentId, int userId)
    {
        return await _commentRepository.IsLikedByUserAsync(commentId, userId);
    }

    public async Task<int> GetLikeCountAsync(Guid commentId)
    {
        return await _commentRepository.GetLikeCountAsync(commentId);
    }
}

