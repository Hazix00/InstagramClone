using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

[Table("comments")]
public class Comment : BaseEntity<Guid>
{
    public Comment()
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

    [Column("parent_comment_id")]
    public Guid? ParentCommentId { get; set; }

    [ForeignKey(nameof(ParentCommentId))]
    public Comment? ParentComment { get; set; }

    public ICollection<Comment> Replies { get; set; } = new List<Comment>();

    [Required]
    [MaxLength(1000)]
    [Column("content")]
    public string Content { get; set; } = string.Empty;
}

