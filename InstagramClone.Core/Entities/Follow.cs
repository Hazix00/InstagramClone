using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

[Table("follows")]
public class Follow : BaseEntity<Guid>
{
    public Follow()
    {
        Id = Guid.NewGuid();
    }

    [Required]
    [Column("follower_id")]
    public int FollowerId { get; set; }

    [ForeignKey(nameof(FollowerId))]
    public User Follower { get; set; } = default!;

    [Required]
    [Column("followee_id")]
    public int FolloweeId { get; set; }

    [ForeignKey(nameof(FolloweeId))]
    public User Followee { get; set; } = default!;
}

