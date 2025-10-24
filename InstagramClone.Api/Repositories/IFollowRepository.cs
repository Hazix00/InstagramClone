using InstagramClone.Core.Entities;

namespace InstagramClone.Api.Repositories;

public interface IFollowRepository : IRepository<Follow>
{
    Task<Follow?> GetAsync(int followerId, int followeeId);
    Task<bool> IsFollowingAsync(int followerId, int followeeId);
    Task<List<int>> GetFolloweeIdsAsync(int followerId);
    Task<int> GetFollowerCountAsync(int userId);
    Task<int> GetFollowingCountAsync(int userId);
}

