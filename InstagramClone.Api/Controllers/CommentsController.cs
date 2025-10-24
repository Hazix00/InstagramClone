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
public class CommentsController : ControllerBase
{
    private readonly ICommentService _commentService;
    private readonly IPostService _postService;
    private readonly IUserRepository _userRepository;

    public CommentsController(ICommentService commentService, IPostService postService, IUserRepository userRepository)
    {
        _commentService = commentService;
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

    // List top-level comments for a post with pagination
    [HttpGet("post/{postId:guid}")]
    public async Task<ActionResult<object>> GetCommentsForPost(Guid postId, [FromQuery] int take = 20, [FromQuery] int skip = 0)
    {
        var postExists = await _postService.ExistsAsync(postId);
        if (!postExists) return NotFound("Post not found.");

        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(skip, 0);

        var (total, comments) = await _commentService.GetTopLevelCommentsAsync(postId, take, skip);

        var me = await GetCurrentUserAsync();
        var currentUserId = me?.Id ?? 0;

        var commentDtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            var user = await _userRepository.GetByIdAsync(comment.UserId);
            var likeCount = await _commentService.GetLikeCountAsync(comment.Id);
            var isLiked = await _commentService.IsLikedByUserAsync(comment.Id, currentUserId);
            var replyCount = await _commentService.GetReplyCountAsync(comment.Id);

            commentDtos.Add(new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                Username = user?.Username ?? "Unknown",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                LikeCount = likeCount,
                IsLiked = isLiked,
                ReplyCount = replyCount
            });
        }

        return Ok(new { total, items = commentDtos });
    }

    // List replies for a comment with pagination (on-demand)
    [HttpGet("replies/{parentId:guid}")]
    public async Task<ActionResult<object>> GetRepliesForComment(Guid parentId, [FromQuery] int take = 20, [FromQuery] int skip = 0)
    {
        var parentCommentExists = await _commentService.ExistsAsync(parentId);
        if (!parentCommentExists) return NotFound("Parent comment not found.");

        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(skip, 0);

        var (total, replies) = await _commentService.GetRepliesAsync(parentId, take, skip);

        var me = await GetCurrentUserAsync();
        var currentUserId = me?.Id ?? 0;

        var replyDtos = new List<CommentDto>();
        foreach (var reply in replies)
        {
            var user = await _userRepository.GetByIdAsync(reply.UserId);
            var likeCount = await _commentService.GetLikeCountAsync(reply.Id);
            var isLiked = await _commentService.IsLikedByUserAsync(reply.Id, currentUserId);

            replyDtos.Add(new CommentDto
            {
                Id = reply.Id,
                PostId = reply.PostId,
                Username = user?.Username ?? "Unknown",
                Content = reply.Content,
                CreatedAt = reply.CreatedAt,
                LikeCount = likeCount,
                IsLiked = isLiked,
                ReplyCount = 0 // Replies don't have further replies in this model
            });
        }

        return Ok(new { total, items = replyDtos });
    }

    // Create comment or reply
    [HttpPost("post/{postId:guid}")]
    public async Task<ActionResult<CommentDto>> Create(Guid postId, [FromBody] CommentRequest request)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();
        if (!ModelState.IsValid) return BadRequest(ModelState);

        var postExists = await _postService.ExistsAsync(postId);
        if (!postExists) return NotFound("Post not found.");

        if (request.ParentCommentId.HasValue)
        {
            var parentComment = await _commentService.GetByIdAsync(request.ParentCommentId.Value);
            if (parentComment is null) return NotFound("Parent comment not found.");
            if (parentComment.ParentCommentId.HasValue) return BadRequest("Cannot reply to a reply. Only one level of replies is supported.");
        }

        var comment = new Comment
        {
            PostId = postId,
            UserId = me.Id,
            ParentCommentId = request.ParentCommentId,
            Content = request.Content,
            CreatedAt = DateTime.UtcNow
        };

        await _commentService.CreateAsync(comment);

        var likeCount = await _commentService.GetLikeCountAsync(comment.Id);
        var isLiked = await _commentService.IsLikedByUserAsync(comment.Id, me.Id);
        var replyCount = await _commentService.GetReplyCountAsync(comment.Id);

        var dto = new CommentDto
        {
            Id = comment.Id,
            PostId = comment.PostId,
            Username = me.Username,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            LikeCount = likeCount,
            IsLiked = isLiked,
            ReplyCount = replyCount
        };

        return CreatedAtAction(nameof(GetCommentsForPost), new { postId = comment.PostId }, dto);
    }

    // Delete comment (cascade deletes replies via FK)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var comment = await _commentService.GetByIdAsync(id);
        if (comment is null) return NotFound();
        if (comment.UserId != me.Id) return Forbid();

        await _commentService.DeleteAsync(id);
        return NoContent();
    }

    // Like comment
    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> Like(Guid id)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        var success = await _commentService.LikeCommentAsync(id, me.Id);
        if (!success) return NotFound();

        return NoContent();
    }

    // Unlike comment
    [HttpDelete("{id:guid}/like")]
    public async Task<IActionResult> Unlike(Guid id)
    {
        var me = await GetCurrentUserAsync();
        if (me is null) return Unauthorized();

        await _commentService.UnlikeCommentAsync(id, me.Id);
        return NoContent();
    }
}
