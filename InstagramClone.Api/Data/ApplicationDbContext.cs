using InstagramClone.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace InstagramClone.Api.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<Follow> Follows => Set<Follow>();
    public DbSet<Comment> Comments => Set<Comment>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();
    public DbSet<CommentLike> CommentLikes => Set<CommentLike>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configure User entity
        modelBuilder.Entity<User>(entity =>
        {
            // Create unique index on username
            entity.HasIndex(e => e.Username)
                .IsUnique()
                .HasDatabaseName("ix_users_username");

            // Create unique index on email
            entity.HasIndex(e => e.Email)
                .IsUnique()
                .HasDatabaseName("ix_users_email");

            // Set default value for CreatedAt
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("CURRENT_TIMESTAMP");
        });

        modelBuilder.Entity<Post>()
            .HasIndex(p => p.UserId);

        modelBuilder.Entity<Post>()
            .HasOne(p => p.User)
            .WithMany()
            .HasForeignKey(p => p.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // Follow now has a surrogate key (Id), add unique constraint on follower/followee pair
        modelBuilder.Entity<Follow>()
            .HasIndex(f => new { f.FollowerId, f.FolloweeId })
            .IsUnique();

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Follower)
            .WithMany()
            .HasForeignKey(f => f.FollowerId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Follow>()
            .HasOne(f => f.Followee)
            .WithMany()
            .HasForeignKey(f => f.FolloweeId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Follow>()
            .HasIndex(f => f.FollowerId);

        modelBuilder.Entity<Follow>()
            .HasIndex(f => f.FolloweeId);

        // Comments (self-reference one level for replies)
        modelBuilder.Entity<Comment>()
            .HasOne(c => c.Post)
            .WithMany()
            .HasForeignKey(c => c.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.User)
            .WithMany()
            .HasForeignKey(c => c.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasOne(c => c.ParentComment)
            .WithMany(c => c!.Replies)
            .HasForeignKey(c => c.ParentCommentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.PostId);

        modelBuilder.Entity<Comment>()
            .HasIndex(c => c.ParentCommentId);

        // PostLike now has a surrogate key (Id), add unique constraint on post/user pair
        modelBuilder.Entity<PostLike>()
            .HasIndex(pl => new { pl.PostId, pl.UserId })
            .IsUnique();

        modelBuilder.Entity<PostLike>()
            .HasOne(pl => pl.Post)
            .WithMany()
            .HasForeignKey(pl => pl.PostId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<PostLike>()
            .HasOne(pl => pl.User)
            .WithMany()
            .HasForeignKey(pl => pl.UserId)
            .OnDelete(DeleteBehavior.Cascade);

        // CommentLike now has a surrogate key (Id), add unique constraint on comment/user pair
        modelBuilder.Entity<CommentLike>()
            .HasIndex(cl => new { cl.CommentId, cl.UserId })
            .IsUnique();

        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.Comment)
            .WithMany()
            .HasForeignKey(cl => cl.CommentId)
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<CommentLike>()
            .HasOne(cl => cl.User)
            .WithMany()
            .HasForeignKey(cl => cl.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

