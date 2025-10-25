namespace InstagramClone.Core.DTOs;

public record PostDto
{
    public required Guid Id { get; init; }
    public required int UserId { get; init; }
    public required string Username { get; init; }
    public required string ImageUrl { get; init; }
    public string? Caption { get; init; }
    public required DateTime CreatedAt { get; init; }
    public int LikesCount { get; init; }
    public bool IsLikedByCurrentUser { get; init; }
    public int CommentsCount { get; init; }
}

