using InstagramClone.Core.Entities;

namespace InstagramClone.Api.Repositories;

public interface IPostRepository : IRepository<Post>
{
    Task<List<Post>> GetUserPostsAsync(int userId);
    Task<List<Post>> GetFeedAsync(List<int> userIds, int take, int skip);
    Task<PostLike?> GetLikeAsync(Guid postId, int userId);
    Task<bool> IsLikedByUserAsync(Guid postId, int userId);
    Task AddLikeAsync(PostLike like);
    Task RemoveLikeAsync(PostLike like);
    Task<int> GetLikeCountAsync(Guid postId);
}

