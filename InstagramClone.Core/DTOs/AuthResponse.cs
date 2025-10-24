namespace InstagramClone.Core.DTOs;

public record AuthResponse
{
    public required string Token { get; init; }
    public required string Username { get; init; }
    public required string Email { get; init; }
    public required DateTime Expiration { get; init; }
}

