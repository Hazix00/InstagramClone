using InstagramClone.Core.Entities;
using InstagramClone.Api.Repositories;

namespace InstagramClone.Api.Services;

public class PostService : Service<Post>, IPostService
{
    private readonly IPostRepository _postRepository;
    private readonly IFollowRepository _followRepository;

    public PostService(IPostRepository postRepository, IFollowRepository followRepository) 
        : base(postRepository)
    {
        _postRepository = postRepository;
        _followRepository = followRepository;
    }

    public async Task<List<Post>> GetUserPostsAsync(int userId)
    {
        return await _postRepository.GetUserPostsAsync(userId);
    }

    public async Task<List<Post>> GetFeedAsync(int userId, int take, int skip)
    {
        var followeeIds = await _followRepository.GetFolloweeIdsAsync(userId);
        followeeIds.Add(userId); // Include own posts
        
        return await _postRepository.GetFeedAsync(followeeIds, take, skip);
    }

    public async Task<bool> LikePostAsync(Guid postId, int userId)
    {
        if (!await _postRepository.ExistsAsync(postId))
            return false;

        if (await _postRepository.IsLikedByUserAsync(postId, userId))
            return true;

        var like = new PostLike
        {
            PostId = postId,
            UserId = userId,
            CreatedAt = DateTime.UtcNow
        };

        await _postRepository.AddLikeAsync(like);
        await _postRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnlikePostAsync(Guid postId, int userId)
    {
        var like = await _postRepository.GetLikeAsync(postId, userId);
        if (like == null)
            return true;

        await _postRepository.RemoveLikeAsync(like);
        await _postRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsLikedByUserAsync(Guid postId, int userId)
    {
        return await _postRepository.IsLikedByUserAsync(postId, userId);
    }

    public async Task<int> GetLikeCountAsync(Guid postId)
    {
        return await _postRepository.GetLikeCountAsync(postId);
    }
}

