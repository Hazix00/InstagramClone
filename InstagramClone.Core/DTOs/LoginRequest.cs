using System.ComponentModel.DataAnnotations;
using InstagramClone.Core.Resources;

namespace InstagramClone.Core.DTOs;

public record LoginRequest
{
    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_Username))]
    [MinLength(3, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength_Username))]
    [MaxLength(50, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength_Username))]
    public string Username { get; set; } = string.Empty;

    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_Password))]
    [MinLength(6, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MinLength_Password))]
    public string Password { get; set; } = string.Empty;
}

