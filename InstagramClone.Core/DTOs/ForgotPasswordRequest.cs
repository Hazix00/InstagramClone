using System.ComponentModel.DataAnnotations;
using InstagramClone.Core.Resources;

namespace InstagramClone.Core.DTOs;

public record ForgotPasswordRequest
{
    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_Email))]
    [EmailAddress(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Invalid_Email))]
    public string Email { get; set; } = string.Empty;
}

