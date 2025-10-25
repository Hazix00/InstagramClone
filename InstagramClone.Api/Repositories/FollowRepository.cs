using InstagramClone.Api.Data;
using InstagramClone.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstagramClone.Api.Repositories;

public class FollowRepository(ApplicationDbContext context) : Repository<Follow>(context), IFollowRepository
{

    public async Task<Follow?> GetAsync(int followerId, int followeeId) =>
        await _dbSet.FirstOrDefaultAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

    public async Task<bool> IsFollowingAsync(int followerId, int followeeId) =>
        await _dbSet.AnyAsync(f => f.FollowerId == followerId && f.FolloweeId == followeeId);

    public async Task<List<int>> GetFolloweeIdsAsync(int followerId) =>
        await _dbSet
            .Where(f => f.FollowerId == followerId)
            .Select(f => f.FolloweeId)
            .ToListAsync();

    public async Task<int> GetFollowerCountAsync(int userId) =>
        await _dbSet.CountAsync(f => f.FolloweeId == userId);

    public async Task<int> GetFollowingCountAsync(int userId) =>
        await _dbSet.CountAsync(f => f.FollowerId == userId);
}

