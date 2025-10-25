using InstagramClone.Core.Entities;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;

namespace InstagramClone.Api.Data;

public class DatabaseSeeder(ApplicationDbContext context)
{
    private readonly ApplicationDbContext _context = context;
    private readonly Random _random = new();
    
    // Sample data for generating realistic content
    private readonly string[] _firstNames = {
        "Emma", "Liam", "Olivia", "Noah", "Ava", "Ethan", "Sophia", "Mason", "Isabella", "William",
        "Mia", "James", "Charlotte", "Benjamin", "Amelia", "Lucas", "Harper", "Henry", "Evelyn", "Alexander",
        "Abigail", "Michael", "Emily", "Daniel", "Elizabeth", "Matthew", "Sofia", "Joseph", "Avery", "David"
    };
    
    private readonly string[] _lastNames = {
        "Smith", "Johnson", "Williams", "Brown", "Jones", "Garcia", "Miller", "Davis", "Rodriguez", "Martinez",
        "Hernandez", "Lopez", "Gonzalez", "Wilson", "Anderson", "Thomas", "Taylor", "Moore", "Jackson", "Martin",
        "Lee", "Walker", "Hall", "Allen", "Young", "King", "Wright", "Scott", "Torres", "Nguyen"
    };
    
    private readonly string[] _adjectives = {
        "awesome", "cool", "super", "mega", "ultra", "epic", "great", "amazing", "fantastic", "incredible"
    };
    
    private readonly string[] _captions = {
        "Living my best life ✨", "Sunshine and good vibes ☀️", "Making memories 📸",
        "Just another day in paradise 🌴", "Chasing dreams 💫", "Adventure awaits 🌍",
        "Good times and tan lines 🏖️", "Life is beautiful 🌺", "Stay wild 🌿",
        "Coffee and confidence ☕", "Weekend mood 🎉", "Grateful for moments like these 🙏",
        "New day, new adventures 🚀", "Feeling blessed 💖", "Creating my own sunshine 🌞",
        "Living for the moments you can't put into words", "Collect moments, not things",
        "Do more things that make you forget to check your phone", "Life happens, coffee helps",
        "Sunkissed and blessed", "Good vibes only", "Dream big, sparkle more, shine bright"
    };
    
    private readonly string[] _imageUrls = {
        "https://picsum.photos/seed/img1/800/800", "https://picsum.photos/seed/img2/800/800",
        "https://picsum.photos/seed/img3/800/800", "https://picsum.photos/seed/img4/800/800",
        "https://picsum.photos/seed/img5/800/800", "https://picsum.photos/seed/img6/800/800",
        "https://picsum.photos/seed/img7/800/800", "https://picsum.photos/seed/img8/800/800",
        "https://picsum.photos/seed/img9/800/800", "https://picsum.photos/seed/img10/800/800"
    };
    
    private readonly string[] _commentTexts = {
        "Love this! 😍", "Amazing! 🔥", "So cool! 👏", "Beautiful! 💕", "Awesome pic! 📸",
        "Goals! 💯", "Stunning! ✨", "This is everything! 🙌", "So good! 👌", "Perfect! ⭐",
        "Can't stop looking at this!", "Obsessed! 😊", "Pure perfection!", "You're killing it! 🔥",
        "This made my day! 💖", "Incredible shot!", "So inspiring! 🌟", "Wow just wow! 😮"
    };

    public async Task SeedAsync(int userCount = 10000, int postsPerUser = 1)
    {
        Console.WriteLine("🌱 Starting database seeding...");
        
        // Check if already seeded
        if (await _context.Users.AnyAsync())
        {
            Console.WriteLine("⚠️  Database already contains data. Skipping seed.");
            return;
        }

        // Create users
        Console.WriteLine($"Creating {userCount} users...");
        var users = await CreateUsersAsync(userCount);
        
        // Create follows
        Console.WriteLine("Creating random follows...");
        await CreateFollowsAsync(users);
        
        // Create posts with likes and comments
        Console.WriteLine($"Creating {userCount * postsPerUser} posts...");
        await CreatePostsAsync(users, postsPerUser);
        
        Console.WriteLine("✅ Database seeding completed successfully!");
    }

    private async Task<List<User>> CreateUsersAsync(int count)
    {
        var users = new List<User>();
        var usedUsernames = new HashSet<string>();
        var usedEmails = new HashSet<string>();
        
        for (int i = 0; i < count; i++)
        {
            string username;
            string email;
            
            // Generate unique username
            do
            {
                var firstName = _firstNames[_random.Next(_firstNames.Length)];
                var lastName = _lastNames[_random.Next(_lastNames.Length)];
                var suffix = _random.Next(1000, 9999);
                
                username = _random.Next(3) switch
                {
                    0 => $"{firstName.ToLower()}{lastName.ToLower()}{suffix}",
                    1 => $"{firstName.ToLower()}_{_adjectives[_random.Next(_adjectives.Length)]}{suffix}",
                    _ => $"{firstName.ToLower()}.{lastName.ToLower()}{suffix}"
                };
            } while (usedUsernames.Contains(username));
            
            usedUsernames.Add(username);
            
            // Generate unique email
            email = $"{username}@example.com";
            usedEmails.Add(email);
            
            var user = new User
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword("Password123!"), // Same password for all test users
                IsEmailVerified = _random.Next(10) > 2, // 80% verified
                CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365))
            };
            
            users.Add(user);
            
            if ((i + 1) % 1000 == 0)
            {
                Console.WriteLine($"  Created {i + 1}/{count} users...");
            }
        }
        
        await _context.Users.AddRangeAsync(users);
        await _context.SaveChangesAsync();
        
        Console.WriteLine($"✓ Created {users.Count} users");
        return users;
    }

    private async Task CreateFollowsAsync(List<User> users)
    {
        var follows = new List<Follow>();
        var followPairs = new HashSet<string>();
        
        // Each user follows 10-100 random users
        foreach (var user in users)
        {
            var followCount = _random.Next(10, 101);
            var followed = 0;
            
            while (followed < followCount)
            {
                var targetUser = users[_random.Next(users.Count)];
                
                // Don't follow yourself
                if (targetUser.Id == user.Id)
                    continue;
                
                var pair = $"{user.Id}-{targetUser.Id}";
                if (followPairs.Contains(pair))
                    continue;
                
                followPairs.Add(pair);
                follows.Add(new Follow
                {
                    FollowerId = user.Id,
                    FolloweeId = targetUser.Id,
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 365))
                });
                
                followed++;
            }
        }
        
        // Batch insert
        const int batchSize = 5000;
        for (int i = 0; i < follows.Count; i += batchSize)
        {
            var batch = follows.Skip(i).Take(batchSize).ToList();
            await _context.Follows.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            Console.WriteLine($"  Inserted {Math.Min(i + batchSize, follows.Count)}/{follows.Count} follows...");
        }
        
        Console.WriteLine($"✓ Created {follows.Count} follow relationships");
    }

    private async Task CreatePostsAsync(List<User> users, int postsPerUser)
    {
        var posts = new List<Post>();
        var postLikes = new List<PostLike>();
        var comments = new List<Comment>();
        var commentLikes = new List<CommentLike>();
        
        foreach (var user in users)
        {
            for (int p = 0; p < postsPerUser; p++)
            {
                var post = new Post
                {
                    Id = Guid.NewGuid(),
                    UserId = user.Id,
                    ImageUrl = _imageUrls[_random.Next(_imageUrls.Length)] + $"?user={user.Id}&post={p}",
                    Caption = _random.Next(3) == 0 ? null : _captions[_random.Next(_captions.Length)],
                    CreatedAt = DateTime.UtcNow.AddDays(-_random.Next(1, 180))
                };
                
                posts.Add(post);
                
                // Add likes (5-200 likes per post)
                var likeCount = _random.Next(5, 201);
                var likedBy = new HashSet<int>();
                
                for (int l = 0; l < likeCount; l++)
                {
                    var liker = users[_random.Next(users.Count)];
                    if (likedBy.Add(liker.Id))
                    {
                        postLikes.Add(new PostLike
                        {
                            PostId = post.Id,
                            UserId = liker.Id,
                            CreatedAt = post.CreatedAt.AddMinutes(_random.Next(1, 10000))
                        });
                    }
                }
                
                // Add comments (0-20 comments per post)
                var commentCount = _random.Next(0, 21);
                var postComments = new List<Comment>();
                
                for (int c = 0; c < commentCount; c++)
                {
                    var commenter = users[_random.Next(users.Count)];
                    var comment = new Comment
                    {
                        Id = Guid.NewGuid(),
                        PostId = post.Id,
                        UserId = commenter.Id,
                        Content = _commentTexts[_random.Next(_commentTexts.Length)],
                        CreatedAt = post.CreatedAt.AddMinutes(_random.Next(1, 10000))
                    };
                    
                    postComments.Add(comment);
                    comments.Add(comment);
                    
                    // Add comment likes (0-50 per comment)
                    var commentLikeCount = _random.Next(0, 51);
                    var commentLikedBy = new HashSet<int>();
                    
                    for (int cl = 0; cl < commentLikeCount; cl++)
                    {
                        var commentLiker = users[_random.Next(users.Count)];
                        if (commentLikedBy.Add(commentLiker.Id))
                        {
                            commentLikes.Add(new CommentLike
                            {
                                CommentId = comment.Id,
                                UserId = commentLiker.Id,
                                CreatedAt = comment.CreatedAt.AddMinutes(_random.Next(1, 1000))
                            });
                        }
                    }
                }
                
                // Add replies (0-3 replies per comment)
                foreach (var comment in postComments.Take(Math.Min(5, postComments.Count)))
                {
                    var replyCount = _random.Next(0, 4);
                    for (int r = 0; r < replyCount; r++)
                    {
                        var replier = users[_random.Next(users.Count)];
                        var reply = new Comment
                        {
                            Id = Guid.NewGuid(),
                            PostId = post.Id,
                            UserId = replier.Id,
                            ParentCommentId = comment.Id,
                            Content = _commentTexts[_random.Next(_commentTexts.Length)],
                            CreatedAt = comment.CreatedAt.AddMinutes(_random.Next(1, 5000))
                        };
                        
                        comments.Add(reply);
                    }
                }
            }
        }
        
        // Batch insert posts
        Console.WriteLine("  Inserting posts...");
        const int postBatchSize = 1000;
        for (int i = 0; i < posts.Count; i += postBatchSize)
        {
            var batch = posts.Skip(i).Take(postBatchSize).ToList();
            await _context.Posts.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            Console.WriteLine($"    Inserted {Math.Min(i + postBatchSize, posts.Count)}/{posts.Count} posts...");
        }
        
        // Batch insert likes
        Console.WriteLine("  Inserting post likes...");
        const int likeBatchSize = 5000;
        for (int i = 0; i < postLikes.Count; i += likeBatchSize)
        {
            var batch = postLikes.Skip(i).Take(likeBatchSize).ToList();
            await _context.PostLikes.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            Console.WriteLine($"    Inserted {Math.Min(i + likeBatchSize, postLikes.Count)}/{postLikes.Count} likes...");
        }
        
        // Batch insert comments
        Console.WriteLine("  Inserting comments...");
        const int commentBatchSize = 5000;
        for (int i = 0; i < comments.Count; i += commentBatchSize)
        {
            var batch = comments.Skip(i).Take(commentBatchSize).ToList();
            await _context.Comments.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            Console.WriteLine($"    Inserted {Math.Min(i + commentBatchSize, comments.Count)}/{comments.Count} comments...");
        }
        
        // Batch insert comment likes
        Console.WriteLine("  Inserting comment likes...");
        for (int i = 0; i < commentLikes.Count; i += likeBatchSize)
        {
            var batch = commentLikes.Skip(i).Take(likeBatchSize).ToList();
            await _context.CommentLikes.AddRangeAsync(batch);
            await _context.SaveChangesAsync();
            Console.WriteLine($"    Inserted {Math.Min(i + likeBatchSize, commentLikes.Count)}/{commentLikes.Count} comment likes...");
        }
        
        Console.WriteLine($"✓ Created {posts.Count} posts with {postLikes.Count} likes, {comments.Count} comments, and {commentLikes.Count} comment likes");
    }

    private static string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return Convert.ToBase64String(hashedBytes);
    }
}

