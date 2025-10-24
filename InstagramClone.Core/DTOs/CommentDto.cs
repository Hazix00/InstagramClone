namespace InstagramClone.Core.DTOs;

public record CommentDto
{
    public required Guid Id { get; init; }
    public required Guid PostId { get; init; }
    public required string Username { get; init; }
    public required string Content { get; init; }
    public required DateTime CreatedAt { get; init; }
    public required int LikeCount { get; init; }
    public required bool IsLiked { get; init; }
    public required int ReplyCount { get; init; }
}

