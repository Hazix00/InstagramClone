using InstagramClone.Core.Entities;

namespace InstagramClone.Api.Repositories;

public interface ICommentRepository : IRepository<Comment>
{
    Task<(int total, List<Comment> items)> GetTopLevelCommentsAsync(Guid postId, int take, int skip);
    Task<(int total, List<Comment> items)> GetRepliesAsync(Guid parentId, int take, int skip);
    Task<int> GetReplyCountAsync(Guid commentId);
    Task<CommentLike?> GetLikeAsync(Guid commentId, int userId);
    Task<bool> IsLikedByUserAsync(Guid commentId, int userId);
    Task AddLikeAsync(CommentLike like);
    Task RemoveLikeAsync(CommentLike like);
    Task<int> GetLikeCountAsync(Guid commentId);
}

