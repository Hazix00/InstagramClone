using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace InstagramClone.Core.Entities;

public abstract class BaseEntity<TKey>
{
    [Key]
    [Column("id")]
    public TKey Id { get; set; } = default!;

    [Column("created_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [Column("updated_at")]
    public DateTime? UpdatedAt { get; set; }
}

