using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

[Table("posts")]
public class Post : BaseEntity<Guid>
{
    public Post()
    {
        Id = Guid.NewGuid();
    }

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;

    [Required]
    [MaxLength(2048)]
    [Column("image_url")]
    public string ImageUrl { get; set; } = string.Empty;

    [MaxLength(2200)]
    [Column("caption")]
    public string? Caption { get; set; }

    // Navigation properties
    public ICollection<PostLike> PostLikes { get; set; } = new List<PostLike>();
    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
}

