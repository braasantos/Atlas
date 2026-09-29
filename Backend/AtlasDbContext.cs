using Atlas.Models;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Data;

public sealed class AtlasDbContext(DbContextOptions<AtlasDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(user => user.Email).IsUnique();
        modelBuilder.Entity<User>().HasIndex(user => user.Username).IsUnique();
        modelBuilder.Entity<RefreshToken>().HasIndex(token => token.TokenHash).IsUnique();
        modelBuilder.Entity<RefreshToken>()
            .HasOne(token => token.User)
            .WithMany()
            .HasForeignKey(token => token.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

public sealed class RefreshToken
{
    public int Id { get; set; }
    public string TokenHash { get; set; } = string.Empty;
    public int UserId { get; set; }
    public User User { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }
}
