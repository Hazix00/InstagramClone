using InstagramClone.Core.Entities;

namespace InstagramClone.Api.Services;

public interface ICommentService : IService<Comment>
{
    Task<(int total, List<Comment> items)> GetTopLevelCommentsAsync(Guid postId, int take, int skip);
    Task<(int total, List<Comment> items)> GetRepliesAsync(Guid parentId, int take, int skip);
    Task<int> GetReplyCountAsync(Guid commentId);
    Task<Comment?> CreateCommentAsync(Guid postId, int userId, string content, Guid? parentCommentId);
    Task<bool> DeleteCommentAsync(Guid commentId, int userId);
    Task<bool> LikeCommentAsync(Guid commentId, int userId);
    Task<bool> UnlikeCommentAsync(Guid commentId, int userId);
    Task<bool> IsLikedByUserAsync(Guid commentId, int userId);
    Task<int> GetLikeCountAsync(Guid commentId);
}

