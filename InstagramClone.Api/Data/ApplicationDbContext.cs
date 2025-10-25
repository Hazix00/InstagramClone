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

        // Configure Post entity
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasIndex(p => p.UserId);

            // Post -> User relationship
            entity.HasOne(p => p.User)
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Post -> PostLikes relationship (one-to-many)
            entity.HasMany(p => p.PostLikes)
                .WithOne(pl => pl.Post)
                .HasForeignKey(pl => pl.PostId)
                .OnDelete(DeleteBehavior.Cascade);

            // Post -> Comments relationship (one-to-many)
            entity.HasMany(p => p.Comments)
                .WithOne(c => c.Post)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade);
        });

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

        // Configure Comment entity
        modelBuilder.Entity<Comment>(entity =>
        {
            entity.HasIndex(c => c.PostId);
            entity.HasIndex(c => c.ParentCommentId);

            // Comment -> User relationship
            entity.HasOne(c => c.User)
                .WithMany()
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            // Comment self-reference for replies (one level deep)
            entity.HasOne(c => c.ParentComment)
                .WithMany(c => c!.Replies)
                .HasForeignKey(c => c.ParentCommentId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure PostLike entity
        modelBuilder.Entity<PostLike>(entity =>
        {
            // Unique constraint: one like per user per post
            entity.HasIndex(pl => new { pl.PostId, pl.UserId })
                .IsUnique();

            // PostLike -> User relationship
            entity.HasOne(pl => pl.User)
                .WithMany()
                .HasForeignKey(pl => pl.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        // Configure CommentLike entity
        modelBuilder.Entity<CommentLike>(entity =>
        {
            // Unique constraint: one like per user per comment
            entity.HasIndex(cl => new { cl.CommentId, cl.UserId })
                .IsUnique();

            // CommentLike -> Comment relationship
            entity.HasOne(cl => cl.Comment)
                .WithMany()
                .HasForeignKey(cl => cl.CommentId)
                .OnDelete(DeleteBehavior.Cascade);

            // CommentLike -> User relationship
            entity.HasOne(cl => cl.User)
                .WithMany()
                .HasForeignKey(cl => cl.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });
    }
}

