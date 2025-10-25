using InstagramClone.Api.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

// Seeder CLI: dotnet run --project InstagramClone.Seeder -- --users 10000 --posts 1

var builder = Host.CreateApplicationBuilder(args);

// Load configuration from API appsettings.json to reuse connection string
builder.Configuration
    .AddJsonFile(Path.Combine(AppContext.BaseDirectory, "..", "InstagramClone.Api", "appsettings.json"), optional: true)
    .AddEnvironmentVariables();

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? Environment.GetEnvironmentVariable("DefaultConnection")
    ?? throw new InvalidOperationException("Connection string not found");

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// Reuse the DatabaseSeeder from API project
builder.Services.AddScoped<DatabaseSeeder>();

var host = builder.Build();

// Parse CLI args
int users = GetArg("--users", 10000);
int posts = GetArg("--posts", 1);

Console.WriteLine($"🌱 Seeding with users={users}, postsPerUser={posts}...");

using (var scope = host.Services.CreateScope())
{
    var seeder = scope.ServiceProvider.GetRequiredService<DatabaseSeeder>();
    await seeder.SeedAsync(users, posts);
}

Console.WriteLine("✅ Seeding completed");
return;

static int GetArg(string name, int defaultValue)
{
    var args = Environment.GetCommandLineArgs();
    var idx = Array.FindIndex(args, a => a.Equals(name, StringComparison.OrdinalIgnoreCase));
    if (idx >= 0 && idx + 1 < args.Length && int.TryParse(args[idx + 1], out var val))
    {
        return val;
    }
    return defaultValue;
}
