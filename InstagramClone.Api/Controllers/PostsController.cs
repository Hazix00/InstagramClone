using InstagramClone.Api.Attributes;
using InstagramClone.Api.Data;
using InstagramClone.Api.Services;
using InstagramClone.Core.Entities;
using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InstagramClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController(IPostService postService, ApplicationDbContext context) : ControllerBase
{
    private readonly IPostService _postService = postService;
    private readonly ApplicationDbContext _context = context;

    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetMyPosts([CurrentUser] User me)
    {
        var posts = await _postService.GetUserPostsAsync(me.Id);
        
        var postDtos = posts.Select(p => new PostDto
        {
            Id = p.Id,
            UserId = p.UserId,
            Username = me.Username,
            ImageUrl = p.ImageUrl,
            Caption = p.Caption,
            CreatedAt = p.CreatedAt,
            LikesCount = _context.PostLikes.Count(pl => pl.PostId == p.Id),
            IsLikedByCurrentUser = _context.PostLikes.Any(pl => pl.PostId == p.Id && pl.UserId == me.Id),
            CommentsCount = _context.Comments.Count(c => c.PostId == p.Id && c.ParentCommentId == null)
        });

        return Ok(postDtos);
    }

    [HttpGet("feed")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetFeed([CurrentUser] User me, [FromQuery] int take = 5, [FromQuery] int skip = 0)
    {

        // Get posts with related data
        var posts = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.PostLikes)
            .OrderByDescending(p => p.CreatedAt)
            .Skip(skip)
            .Take(take)
            .ToListAsync();
        
        var postDtos = new List<PostDto>();
        foreach (var post in posts)
        {
            // Get comments count (only top-level comments)
            var commentsCount = await _context.Comments
                .Where(c => c.PostId == post.Id && c.ParentCommentId == null)
                .CountAsync();

            postDtos.Add(new PostDto
            {
                Id = post.Id,
                UserId = post.UserId,
                Username = post.User?.Username ?? "Unknown",
                ImageUrl = post.ImageUrl,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt,
                LikesCount = post.PostLikes.Count,
                IsLikedByCurrentUser = post.PostLikes.Any(pl => pl.UserId == me.Id),
                CommentsCount = commentsCount
            });
        }

        return Ok(postDtos);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<PostDto>> GetById([CurrentUser] User me, Guid id)
    {

        var post = await _context.Posts
            .Include(p => p.User)
            .Include(p => p.PostLikes)
            .FirstOrDefaultAsync(p => p.Id == id);

        if (post is null) return NotFound();

        var commentsCount = await _context.Comments
            .Where(c => c.PostId == post.Id && c.ParentCommentId == null)
            .CountAsync();

        var dto = new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Username = post.User?.Username ?? "Unknown",
            ImageUrl = post.ImageUrl,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt,
            LikesCount = post.PostLikes.Count,
            IsLikedByCurrentUser = post.PostLikes.Any(pl => pl.UserId == me.Id),
            CommentsCount = commentsCount
        };

        return Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([CurrentUser] User me, [FromBody] CreatePostRequest request)
    {
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var post = new Post
        {
            UserId = me.Id,
            ImageUrl = request.ImageUrl,
            Caption = request.Caption,
            CreatedAt = DateTime.UtcNow
        };

        await _postService.CreateAsync(post);

        var dto = new PostDto
        {
            Id = post.Id,
            UserId = post.UserId,
            Username = me.Username,
            ImageUrl = post.ImageUrl,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt,
            LikesCount = 0,
            IsLikedByCurrentUser = false,
            CommentsCount = 0
        };

        return CreatedAtAction(nameof(GetMyPosts), new { id = post.Id }, dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([CurrentUser] User me, Guid id)
    {

        var post = await _postService.GetByIdAsync(id);
        if (post is null) return NotFound();
        if (post.UserId != me.Id) return Forbid();

        await _postService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> Like([CurrentUser] User me, Guid id)
    {

        var success = await _postService.LikePostAsync(id, me.Id);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}/like")]
    public async Task<IActionResult> Unlike([CurrentUser] User me, Guid id)
    {

        await _postService.UnlikePostAsync(id, me.Id);
        return NoContent();
    }
}


