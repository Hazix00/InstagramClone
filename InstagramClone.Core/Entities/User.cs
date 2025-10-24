using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

[Table("users")]
public class User : BaseEntity<int>
{
    [Required]
    [MaxLength(50)]
    [Column("username")]
    public string Username { get; set; } = string.Empty;

    [Required]
    [MaxLength(255)]
    [EmailAddress]
    [Column("email")]
    public string Email { get; set; } = string.Empty;

    [Required]
    [MaxLength(500)]
    [Column("password_hash")]
    public string PasswordHash { get; set; } = string.Empty;

    [Column("is_email_verified")]
    public bool IsEmailVerified { get; set; } = false;

    [MaxLength(500)]
    [Column("email_verification_token")]
    public string? EmailVerificationToken { get; set; }

    [Column("email_verification_token_expires")]
    public DateTime? EmailVerificationTokenExpires { get; set; }

    [MaxLength(500)]
    [Column("password_reset_token")]
    public string? PasswordResetToken { get; set; }

    [Column("password_reset_token_expires")]
    public DateTime? PasswordResetTokenExpires { get; set; }
}

