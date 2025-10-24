using InstagramClone.Core.Entities;
using InstagramClone.Api.Repositories;

namespace InstagramClone.Api.Services;

public class FollowService : Service<Follow>, IFollowService
{
    private readonly IFollowRepository _followRepository;
    private readonly IUserRepository _userRepository;

    public FollowService(IFollowRepository followRepository, IUserRepository userRepository) 
        : base(followRepository)
    {
        _followRepository = followRepository;
        _userRepository = userRepository;
    }

    public async Task<bool> FollowUserAsync(int followerId, int followeeId)
    {
        // Cannot follow yourself
        if (followerId == followeeId)
            return false;

        // Check if target user exists
        if (!await _userRepository.ExistsAsync(followeeId))
            return false;

        // Check if already following
        if (await _followRepository.IsFollowingAsync(followerId, followeeId))
            return true;

        var follow = new Follow
        {
            FollowerId = followerId,
            FolloweeId = followeeId,
            CreatedAt = DateTime.UtcNow
        };

        await _followRepository.AddAsync(follow);
        await _followRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> UnfollowUserAsync(int followerId, int followeeId)
    {
        var follow = await _followRepository.GetAsync(followerId, followeeId);
        if (follow == null)
            return true;

        await _followRepository.DeleteAsync(follow);
        await _followRepository.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsFollowingAsync(int followerId, int followeeId)
    {
        return await _followRepository.IsFollowingAsync(followerId, followeeId);
    }

    public async Task<int> GetFollowerCountAsync(int userId)
    {
        return await _followRepository.GetFollowerCountAsync(userId);
    }

    public async Task<int> GetFollowingCountAsync(int userId)
    {
        return await _followRepository.GetFollowingCountAsync(userId);
    }
}

