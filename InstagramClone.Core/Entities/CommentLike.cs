using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

[Table("comment_likes")]
public class CommentLike : BaseEntity<Guid>
{
    public CommentLike()
    {
        Id = Guid.NewGuid();
    }

    [Required]
    [Column("comment_id")]
    public Guid CommentId { get; set; }

    [ForeignKey(nameof(CommentId))]
    public Comment Comment { get; set; } = default!;

    [Required]
    [Column("user_id")]
    public int UserId { get; set; }

    [ForeignKey(nameof(UserId))]
    public User User { get; set; } = default!;
}

