# InstagramClone.Seeder

Professional database seeding tool for the Instagram Clone application.

## Overview

This is a standalone console application designed to populate your development database with realistic test data. It follows industry best practices by separating seeding logic from the main API runtime.

## Features

- 🌱 Seeds users, posts, comments, likes, and follows
- 📊 Generates realistic data using Faker.Net
- 🚀 Batch inserts for optimal performance
- ⚙️ Configurable via command-line arguments
- 🔧 Reuses API's `DatabaseSeeder` logic
- 🗄️ Auto-reads connection string from API `appsettings.json`

## Usage

### From Command Line

```bash
# Default: 10,000 users, 1 post each
dotnet run --project InstagramClone.Seeder -- --users 10000 --posts 1

# Small dataset for quick testing
dotnet run --project InstagramClone.Seeder -- --users 100 --posts 2

# Large dataset
dotnet run --project InstagramClone.Seeder -- --users 50000 --posts 3
```

### From VS Code

Use the built-in tasks:
1. Press `Ctrl+Shift+P` (or `Cmd+Shift+P` on Mac)
2. Select "Tasks: Run Task"
3. Choose one of:
   - **Seed Database (10k users)** - Standard seeding
   - **Seed Database (small - 100 users)** - Quick testing
   - **Seed Database (custom)** - Enter your own values

## Configuration

### Connection String

The seeder reads the connection string from:
1. API's `appsettings.json` (`ConnectionStrings:DefaultConnection`)
2. Environment variable `DefaultConnection`

### Command-Line Arguments

| Argument | Default | Description |
|----------|---------|-------------|
| `--users` | 10000 | Number of users to create |
| `--posts` | 1 | Number of posts per user |

## What Gets Seeded

1. **Users**: Realistic usernames, emails, passwords (all use `Pa$$w0rd`)
2. **Posts**: Random images from Lorem Picsum, captions
3. **Comments**: Top-level comments and replies (one level deep)
4. **Likes**: Random post likes and comment likes
5. **Follows**: Random follow relationships

## Performance

- Uses EF Core batch inserts
- Efficient LINQ queries
- ~10,000 users with posts/comments/likes seeds in ~30-60 seconds (depending on hardware)

## Architecture

```
InstagramClone.Seeder (Console App)
├── References InstagramClone.Api (for DbContext)
├── References InstagramClone.Core (for Entities)
└── Reuses DatabaseSeeder from API
```

## Design Patterns

- **Separation of Concerns**: Seeding is isolated from runtime API
- **Configuration Management**: Reuses API configuration
- **Dependency Injection**: Uses .NET Generic Host for DI
- **IDesignTimeDbContextFactory**: API implements this for EF tooling

## Production Notes

❌ **DO NOT** run this in production environments!

This tool is designed for:
- Local development
- CI/CD test environments
- Demo/staging environments

## Troubleshooting

### Error: "Connection string not found"

Ensure `InstagramClone.Api/appsettings.json` contains a valid `ConnectionStrings:DefaultConnection` or set the `DefaultConnection` environment variable.

### Error: "Assembly version conflict"

Run `dotnet clean` and `dotnet build` at the solution level to ensure all projects use consistent package versions.

### Slow Performance

- Check your database connection latency
- Reduce the `--users` count
- Ensure PostgreSQL has adequate resources

## Contributing

When adding new entities to the domain:
1. Update `DatabaseSeeder` in the API project
2. This tool will automatically use the updated logic

