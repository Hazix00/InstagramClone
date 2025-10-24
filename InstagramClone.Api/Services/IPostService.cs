using InstagramClone.Core.Entities;

namespace InstagramClone.Api.Services;

public interface IPostService : IService<Post>
{
    Task<List<Post>> GetUserPostsAsync(int userId);
    Task<List<Post>> GetFeedAsync(int userId, int take, int skip);
    Task<bool> LikePostAsync(Guid postId, int userId);
    Task<bool> UnlikePostAsync(Guid postId, int userId);
    Task<bool> IsLikedByUserAsync(Guid postId, int userId);
    Task<int> GetLikeCountAsync(Guid postId);
}

