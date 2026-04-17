using Microsoft.EntityFrameworkCore;
using MicoHoodApi.Entities;

namespace MicoHoodApi.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users => Set<User>();
    public DbSet<Post> Posts => Set<Post>();
    public DbSet<PostLike> PostLikes => Set<PostLike>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User
        modelBuilder.Entity<User>(e =>
        {
            e.HasKey(u => u.Id);
            e.HasIndex(u => u.Email).IsUnique();
            e.HasIndex(u => u.Username).IsUnique();
            e.Property(u => u.Email).IsRequired().HasMaxLength(256);
            e.Property(u => u.Username).IsRequired().HasMaxLength(100);
            e.Property(u => u.PasswordHash).IsRequired();
        });

        // Post
        modelBuilder.Entity<Post>(e =>
        {
            e.HasKey(p => p.Id);
            e.Property(p => p.Title).IsRequired().HasMaxLength(200);
            e.Property(p => p.Content).IsRequired();
            e.Property(p => p.Location).IsRequired().HasMaxLength(100);
            e.HasOne(p => p.User)
             .WithMany(u => u.Posts)
             .HasForeignKey(p => p.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });

        // PostLike - unique constraint: a user can only like a post once
        modelBuilder.Entity<PostLike>(e =>
        {
            e.HasKey(pl => pl.Id);
            e.HasIndex(pl => new { pl.PostId, pl.UserId }).IsUnique();
            e.HasOne(pl => pl.Post)
             .WithMany(p => p.Likes)
             .HasForeignKey(pl => pl.PostId)
             .OnDelete(DeleteBehavior.Cascade);
            e.HasOne(pl => pl.User)
             .WithMany(u => u.Likes)
             .HasForeignKey(pl => pl.UserId)
             .OnDelete(DeleteBehavior.Cascade);
        });
    }
}
