using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace InstagramClone.Api.Data;

public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        // Read connection string from environment or fallback
        var connectionString = Environment.GetEnvironmentVariable("DefaultConnection")
            ?? "Host=localhost;Database=instagram_clone;Username=postgres;Password=;Include Error Detail=true";

        optionsBuilder.UseNpgsql(connectionString);
        return new ApplicationDbContext(optionsBuilder.Options);
    }
}


