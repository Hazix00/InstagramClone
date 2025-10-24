using System.ComponentModel.DataAnnotations;
using InstagramClone.Core.Resources;

namespace InstagramClone.Core.DTOs;

public record RegisterRequest
{
    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_Username))]
    [MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength_Username))]
    [MaxLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength_Username))]
    [RegularExpression(@"^[a-zA-Z0-9_]+$", ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid_UsernameFormat))]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_Email))]
    [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid_Email))]
    [MaxLength(255, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength_Email))]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_Password))]
    [MinLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength_Password))]
    [MaxLength(100, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength_Password))]
    public string Password { get; set; } = string.Empty;

   [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_ConfirmPassword))]
   [Compare(nameof(Password), ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Passwords_DoNotMatch))]
    public string ConfirmPassword { get; set; } = string.Empty;
}

