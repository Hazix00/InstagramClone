using InstagramClone.Api.Attributes;
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
public class CommentsController(ICommentService commentService, IPostService postService, IUserRepository userRepository) 
    : ControllerBase
{
    private readonly ICommentService _commentService = commentService;
    private readonly IPostService _postService = postService;
    private readonly IUserRepository _userRepository = userRepository;

    // List top-level comments for a post with pagination
    [HttpGet("post/{postId:guid}")]
    public async Task<ActionResult<object>> GetCommentsForPost([CurrentUser] User me, Guid postId, [FromQuery] int take = 20, [FromQuery] int skip = 0)
    {
        var postExists = await _postService.ExistsAsync(postId);
        if (!postExists) return NotFound("Post not found.");

        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(skip, 0);

        var (total, comments) = await _commentService.GetTopLevelCommentsAsync(postId, take, skip);

        var commentDtos = new List<CommentDto>();
        foreach (var comment in comments)
        {
            var user = await _userRepository.GetByIdAsync(comment.UserId);
            var likeCount = await _commentService.GetLikeCountAsync(comment.Id);
            var isLiked = await _commentService.IsLikedByUserAsync(comment.Id, me.Id);
            var replyCount = await _commentService.GetReplyCountAsync(comment.Id);

            commentDtos.Add(new CommentDto
            {
                Id = comment.Id,
                PostId = comment.PostId,
                UserId = comment.UserId,
                Username = user?.Username ?? "Unknown",
                Content = comment.Content,
                CreatedAt = comment.CreatedAt,
                LikesCount = likeCount,
                IsLikedByCurrentUser = isLiked,
                RepliesCount = replyCount,
                ParentCommentId = comment.ParentCommentId
            });
        }

        return Ok(new { total, items = commentDtos });
    }

    // List replies for a comment with pagination (on-demand)
    [HttpGet("replies/{parentId:guid}")]
    public async Task<ActionResult<object>> GetRepliesForComment([CurrentUser] User me, Guid parentId, [FromQuery] int take = 20, [FromQuery] int skip = 0)
    {
        var parentCommentExists = await _commentService.ExistsAsync(parentId);
        if (!parentCommentExists) return NotFound("Parent comment not found.");

        take = Math.Clamp(take, 1, 100);
        skip = Math.Max(skip, 0);

        var (total, replies) = await _commentService.GetRepliesAsync(parentId, take, skip);

        var replyDtos = new List<CommentDto>();
        foreach (var reply in replies)
        {
            var user = await _userRepository.GetByIdAsync(reply.UserId);
            var likeCount = await _commentService.GetLikeCountAsync(reply.Id);
            var isLiked = await _commentService.IsLikedByUserAsync(reply.Id, me.Id);

            replyDtos.Add(new CommentDto
            {
                Id = reply.Id,
                PostId = reply.PostId,
                UserId = reply.UserId,
                Username = user?.Username ?? "Unknown",
                Content = reply.Content,
                CreatedAt = reply.CreatedAt,
                LikesCount = likeCount,
                IsLikedByCurrentUser = isLiked,
                RepliesCount = 0, // Replies don't have further replies in this model
                ParentCommentId = reply.ParentCommentId
            });
        }

        return Ok(new { total, items = replyDtos });
    }

    // Create comment or reply
    [HttpPost("post/{postId:guid}")]
    public async Task<ActionResult<CommentDto>> Create([CurrentUser] User me, Guid postId, [FromBody] CommentRequest request)
    {
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
            UserId = comment.UserId,
            Username = me.Username,
            Content = comment.Content,
            CreatedAt = comment.CreatedAt,
            LikesCount = likeCount,
            IsLikedByCurrentUser = isLiked,
            RepliesCount = replyCount,
            ParentCommentId = comment.ParentCommentId
        };

        return CreatedAtAction(nameof(GetCommentsForPost), new { postId = comment.PostId }, dto);
    }

    // Delete comment (cascade deletes replies via FK)
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete([CurrentUser] User me, Guid id)
    {

        var comment = await _commentService.GetByIdAsync(id);
        if (comment is null) return NotFound();
        if (comment.UserId != me.Id) return Forbid();

        await _commentService.DeleteAsync(id);
        return NoContent();
    }

    // Like comment
    [HttpPost("{id:guid}/like")]
    public async Task<IActionResult> Like([CurrentUser] User me, Guid id)
    {

        var success = await _commentService.LikeCommentAsync(id, me.Id);
        if (!success) return NotFound();

        return NoContent();
    }

    // Unlike comment
    [HttpDelete("{id:guid}/like")]
    public async Task<IActionResult> Unlike([CurrentUser] User me, Guid id)
    {

        await _commentService.UnlikeCommentAsync(id, me.Id);
        return NoContent();
    }
}
