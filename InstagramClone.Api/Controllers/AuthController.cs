using InstagramClone.Api.Data;
using InstagramClone.Api.Services;
using InstagramClone.Core.Entities;
using InstagramClone.Core.DTOs;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InstagramClone.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(ApplicationDbContext context, JwtTokenService tokenService, ILogger<AuthController> logger) : ControllerBase
{
    private readonly ApplicationDbContext _context = context;
    private readonly JwtTokenService _tokenService = tokenService;
    private readonly ILogger<AuthController> _logger = logger;

    [HttpPost("register")]
    public async Task<ActionResult<AuthResponse>> Register([FromBody] RegisterRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Check if username already exists
        if (await _context.Users.AnyAsync(u => u.Username == request.Username))
        {
            return BadRequest(new { message = "Username already exists" });
        }

        // Check if email already exists
        if (await _context.Users.AnyAsync(u => u.Email == request.Email))
        {
            return BadRequest(new { message = "Email already exists" });
        }

        // Create new user
        var user = new User
        {
            Username = request.Username,
            Email = request.Email,
            PasswordHash = HashPassword(request.Password),
            CreatedAt = DateTime.UtcNow
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        // Generate token
        var authResponse = _tokenService.GenerateToken(user);

        _logger.LogInformation("User {Username} (ID: {UserId}) registered successfully", user.Username, user.Id);

        return Ok(authResponse);
    }

    [HttpPost("login")]
    public async Task<ActionResult<AuthResponse>> Login([FromBody] LoginRequest request)
    {
        // Validate model state
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        // Find user by username OR email
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.Username == request.Username || u.Email == request.Username);

        if (user == null || !VerifyPassword(request.Password, user.PasswordHash))
        {
            return Unauthorized(new { message = "Invalid username or password" });
        }

        // Update last login (optional)
        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        // Generate token
        var authResponse = _tokenService.GenerateToken(user);

        _logger.LogInformation("User {Username} (ID: {UserId}) logged in successfully", user.Username, user.Id);

        return Ok(authResponse);
    }

    [HttpPost("forgot-password")]
    public async Task<ActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        // Don't reveal whether user exists or not for security
        if (user == null)
        {
            return Ok(new { message = "If the email exists, a password reset link has been sent." });
        }

        // Generate password reset token
        user.PasswordResetToken = GenerateToken();
        user.PasswordResetTokenExpires = DateTime.UtcNow.AddHours(1);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // TODO: Send email with reset link
        _logger.LogInformation("Password reset requested for user {Email}. Token: {Token}", user.Email, user.PasswordResetToken);

        return Ok(new { message = "If the email exists, a password reset link has been sent." });
    }

    [HttpPost("reset-password")]
    public async Task<ActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == request.Email &&
            u.PasswordResetToken == request.Token &&
            u.PasswordResetTokenExpires > DateTime.UtcNow);

        if (user == null)
        {
            return BadRequest(new { message = "Invalid or expired reset token." });
        }

        // Update password
        user.PasswordHash = HashPassword(request.NewPassword);
        user.PasswordResetToken = null;
        user.PasswordResetTokenExpires = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Password reset successfully for user {Email}", user.Email);

        return Ok(new { message = "Password has been reset successfully." });
    }

    [HttpPost("send-verification-email")]
    public async Task<ActionResult> SendVerificationEmail([FromBody] ForgotPasswordRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == request.Email);

        if (user == null)
        {
            return Ok(new { message = "If the email exists, a verification link has been sent." });
        }

        if (user.IsEmailVerified)
        {
            return BadRequest(new { message = "Email is already verified." });
        }

        // Generate email verification token
        user.EmailVerificationToken = GenerateToken();
        user.EmailVerificationTokenExpires = DateTime.UtcNow.AddDays(1);
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        // TODO: Send verification email
        _logger.LogInformation("Email verification sent for user {Email}. Token: {Token}", user.Email, user.EmailVerificationToken);

        return Ok(new { message = "Verification email has been sent." });
    }

    [HttpPost("verify-email")]
    public async Task<ActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var user = await _context.Users.FirstOrDefaultAsync(u =>
            u.Email == request.Email &&
            u.EmailVerificationToken == request.Token &&
            u.EmailVerificationTokenExpires > DateTime.UtcNow);

        if (user == null)
        {
            return BadRequest(new { message = "Invalid or expired verification token." });
        }

        // Verify email
        user.IsEmailVerified = true;
        user.EmailVerificationToken = null;
        user.EmailVerificationTokenExpires = null;
        user.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        _logger.LogInformation("Email verified successfully for user {Email}", user.Email);

        return Ok(new { message = "Email has been verified successfully." });
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(password);
        var hash = sha256.ComputeHash(bytes);
        return Convert.ToBase64String(hash);
    }

    private static bool VerifyPassword(string password, string hash)
    {
        var passwordHash = HashPassword(password);
        return passwordHash == hash;
    }

    private static string GenerateToken()
    {
        return Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
    }

    [HttpGet("profile/{username}")]
    public async Task<ActionResult<UserProfileDto>> GetProfileByUsername(string username)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);
        
        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        return Ok(new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt
        });
    }

    [HttpGet("profile")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<UserProfileDto>> GetProfile()
    {
        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized();
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound();
        }

        var profile = new UserProfileDto
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            IsEmailVerified = user.IsEmailVerified,
            CreatedAt = user.CreatedAt
        };

        return Ok(profile);
    }

    [HttpPut("profile")]
    [Microsoft.AspNetCore.Authorization.Authorize]
    public async Task<ActionResult<object>> UpdateProfile([FromBody] UpdateProfileRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var userIdClaim = User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;

        if (string.IsNullOrEmpty(userIdClaim) || !int.TryParse(userIdClaim, out int userId))
        {
            return Unauthorized(new { message = "Invalid user token" });
        }

        var user = await _context.Users.FindAsync(userId);

        if (user == null)
        {
            return NotFound(new { message = "User not found" });
        }

        // Check if username is being changed and if it's already taken
        if (user.Username != request.Username)
        {
            if (await _context.Users.AnyAsync(u => u.Username == request.Username && u.Id != userId))
            {
                return BadRequest(new { message = "Username already exists" });
            }
            user.Username = request.Username;
        }

        // Check if email is being changed and if it's already taken
        if (user.Email != request.Email)
        {
            if (await _context.Users.AnyAsync(u => u.Email == request.Email && u.Id != userId))
            {
                return BadRequest(new { message = "Email already exists" });
            }
            user.Email = request.Email;
            user.IsEmailVerified = false; // Reset verification if email changed
        }

        user.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync();

        _logger.LogInformation("User {UserId} updated profile successfully", userId);

        return Ok(new
        {
            id = user.Id,
            username = user.Username,
            email = user.Email,
            isEmailVerified = user.IsEmailVerified,
            message = "Profile updated successfully"
        });
    }
}

