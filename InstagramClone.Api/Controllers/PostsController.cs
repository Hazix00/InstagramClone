using System.Security.Claims;
using InstagramClone.Api.Repositories;
using InstagramClone.Api.Services;
using InstagramClone.Core.Entities;
using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace InstagramClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IPostService _postService;
    private readonly IUserRepository _userRepository;

    public PostsController(IPostService postService, IUserRepository userRepository)
    {
        _postService = postService;
        _userRepository = userRepository;
    }

    private async Task<User?> GetCurrentUserAsync()
    {
        var idClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                   ?? User.FindFirst("sub")?.Value
                   ?? User.FindFirst("uid")?.Value;

        if (int.TryParse(idClaim, out var id))
            return await _userRepository.GetByIdAsync(id);

        var username = User.Identity?.Name
                    ?? User.FindFirst("preferred_username")?.Value
                    ?? User.FindFirst("unique_name")?.Value;

        if (string.IsNullOrWhiteSpace(username)) return null;

        return await _userRepository.GetByUsernameAsync(username);
    }

    [HttpGet("me")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetMyPosts()
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var posts = await _postService.GetUserPostsAsync(me.Id);
        
        var postDtos = posts.Select(p => new PostDto
        {
            Id = p.Id,
            Username = me.Username,
            ImageUrl = p.ImageUrl,
            Caption = p.Caption,
            CreatedAt = p.CreatedAt
        });

        return Ok(postDtos);
    }

    [HttpGet("feed")]
    public async Task<ActionResult<IEnumerable<PostDto>>> GetFeed([FromQuery] int take = 50, [FromQuery] int skip = 0)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var posts = await _postService.GetFeedAsync(me.Id, take, skip);
        
        var postDtos = new List<PostDto>();
        foreach (var post in posts)
        {
            var user = await _userRepository.GetByIdAsync(post.UserId);
            postDtos.Add(new PostDto
            {
                Id = post.Id,
                Username = user?.Username ?? "Unknown",
                ImageUrl = post.ImageUrl,
                Caption = post.Caption,
                CreatedAt = post.CreatedAt
            });
        }

        return Ok(postDtos);
    }

    [HttpPost]
    public async Task<ActionResult<PostDto>> Create([FromBody] CreatePostRequest request)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();
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
            Username = me.Username,
            ImageUrl = post.ImageUrl,
            Caption = post.Caption,
            CreatedAt = post.CreatedAt
        };

        return CreatedAtAction(nameof(GetMyPosts), new { id = post.Id }, dto);
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var post = await _postService.GetByIdAsync(id);
        if (post is null) return NotFound();
        if (post.UserId != me.Id) return Forbid();

        await _postService.DeleteAsync(id);
        return NoContent();
    }

    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> Like(Guid id)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var success = await _postService.LikePostAsync(id, me.Id);
        if (!success) return NotFound();

        return NoContent();
    }

    [HttpDelete("{id:guid}/like")]
    public async Task<IActionResult> Unlike(Guid id)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        await _postService.UnlikePostAsync(id, me.Id);
        return NoContent();
    }
}


