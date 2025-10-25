using InstagramClone.Api.Data;
using InstagramClone.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstagramClone.Api.Repositories;

public class PostRepository(ApplicationDbContext context) : Repository<Post>(context), IPostRepository
{

    public async Task<List<Post>> GetUserPostsAsync(int userId) =>
        await _dbSet
            .Where(p => p.UserId == userId)
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync();

    public async Task<List<Post>> GetFeedAsync(List<int> userIds, int take, int skip) =>
        await _dbSet
            .Where(p => userIds.Contains(p.UserId))
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

    public async Task<PostLike?> GetLikeAsync(Guid postId, int userId) =>
        await _context.PostLikes.FirstOrDefaultAsync(pl => pl.PostId == postId && pl.UserId == userId);

    public async Task<bool> IsLikedByUserAsync(Guid postId, int userId) =>
        await _context.PostLikes.AnyAsync(pl => pl.PostId == postId && pl.UserId == userId);

    public async Task AddLikeAsync(PostLike like)
    {
        await _context.PostLikes.AddAsync(like);
    }

    public async Task RemoveLikeAsync(PostLike like)
    {
        _context.PostLikes.Remove(like);
        await Task.CompletedTask;
    }

    public async Task<int> GetLikeCountAsync(Guid postId) =>
        await _context.PostLikes.CountAsync(pl => pl.PostId == postId);
}

