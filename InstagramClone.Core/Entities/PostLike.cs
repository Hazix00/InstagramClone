using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

[Table("post_likes")]
public class PostLike : BaseEntity<Guid>
{
    public PostLike()
    {
        Id = Guid.NewGuid();
    }

    [Required]
    [Column("post_id")]
    public Guid PostId { get; set; }

    [ForeignKey(nameof(PostId))]
    public Post Post { get; set; } = default!;

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;
}

