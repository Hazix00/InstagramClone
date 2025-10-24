using InstagramClone.Api.Data;
using InstagramClone.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstagramClone.Api.Repositories;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(ApplicationDbContext context) : base(context) { }

    public override async Task<Comment?> GetByIdAsync<TId>(TId id)
    {
        if (id is Guid guidId)
        {
            return await _dbSet
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == guidId);
        }
        return await base.GetByIdAsync(id);
    }

    public async Task<(int total, List<Comment> items)> GetTopLevelCommentsAsync(Guid postId, int take, int skip)
    {
        var query = _dbSet
            .Where(c => c.PostId == postId && c.ParentCommentId == null);

        var total = await query.CountAsync();
        
        var items = await query
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (total, items);
    }

    public async Task<(int total, List<Comment> items)> GetRepliesAsync(Guid parentId, int take, int skip)
    {
        var query = _dbSet.Where(c => c.ParentCommentId == parentId);

        var total = await query.CountAsync();
        
        var items = await query
            .Include(c => c.User)
            .OrderBy(c => c.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();

        return (total, items);
    }

    public async Task<int> GetReplyCountAsync(Guid commentId) =>
        await _dbSet.CountAsync(c => c.ParentCommentId == commentId);

    public async Task<CommentLike?> GetLikeAsync(Guid commentId, int userId) =>
        await _context.CommentLikes.FirstOrDefaultAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

    public async Task<bool> IsLikedByUserAsync(Guid commentId, int userId) =>
        await _context.CommentLikes.AnyAsync(cl => cl.CommentId == commentId && cl.UserId == userId);

    public async Task AddLikeAsync(CommentLike like)
    {
        await _context.CommentLikes.AddAsync(like);
    }

    public async Task RemoveLikeAsync(CommentLike like)
    {
        _context.CommentLikes.Remove(like);
        await Task.CompletedTask;
    }

    public async Task<int> GetLikeCountAsync(Guid commentId) =>
        await _context.CommentLikes.CountAsync(cl => cl.CommentId == commentId);
}

