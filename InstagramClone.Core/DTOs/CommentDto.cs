namespace InstagramClone.Core.DTOs;

public record CommentDto
{
    public required Guid Id { get; init; }
    public required Guid PostId { get; init; }
    public required int UserId { get; init; }
    public required string Username { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedAt { get; init; }
    public int LikesCount { get; init; }
    public bool IsLikedByCurrentUser { get; init; }
    public int RepliesCount { get; init; }
    public Guid? ParentCommentId { get; init; }
}

