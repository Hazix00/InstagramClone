using System.ComponentModel.DataAnnotations;
using InstagramClone.Core.Resources;

namespace InstagramClone.Core.DTOs;

public record CreatePostRequest
{
    [Required(ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.Required_ImageUrl))]
    [MaxLength(2048, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength_ImageUrl))]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(2200, ErrorMessageResourceType = typeof(ValidationMessages), ErrorMessageResourceName = nameof(ValidationMessages.MaxLength_Caption))]
    public string? Caption { get; set; }
}

