namespace InstagramClone.Core.DTOs;

public record UserProfileDto
{
    public required int Id { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required bool IsEmailVerified { get; init; }
    public required DateTime CreatedAt { get; init; }
}

