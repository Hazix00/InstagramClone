namespace InstagramClone.Core.DTOs;

public record PostDto
{
    public required Guid Id { get; init; }
    public required string Username { get; init; }
    public required string ImageUrl { get; init; }
    public string? Caption { get; init; }
    public required DateTime CreatedAt { get; init; }
}

