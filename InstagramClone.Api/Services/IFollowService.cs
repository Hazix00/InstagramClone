using InstagramClone.Core.Entities;

namespace InstagramClone.Api.Services;

public interface IFollowService : IService<Follow>
{
    Task<bool> FollowUserAsync(int followerId, int followeeId);
    Task<bool> UnfollowUserAsync(int followerId, int followeeId);
    Task<bool> IsFollowingAsync(int followerId, int followeeId);
    Task<int> GetFollowerCountAsync(int userId);
    Task<int> GetFollowingCountAsync(int userId);
}

