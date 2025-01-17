using CommunityKnowledgeSharingPlatform.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : IdentityDbContext<Users>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    // DbSets for each table
    public DbSet<Posts> Posts { get; set; }
    public DbSet<Comments> Comments { get; set; }
    public DbSet<Points> Points { get; set; }
    public DbSet<Notifications> Notifications { get; set; }
    public DbSet<Votes> Votes { get; set; }
    public DbSet<Categories> Categories { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<Users>(entity =>
        {

            base.OnModelCreating(modelBuilder);

            // Remove unnecessary columns
            entity.Ignore(e => e.PhoneNumber);
            entity.Ignore(e => e.PhoneNumberConfirmed);
            entity.Ignore(e => e.TwoFactorEnabled);
            entity.Ignore(e => e.LockoutEnd);
            entity.Ignore(e => e.LockoutEnabled);
            entity.Ignore(e => e.AccessFailedCount);
            entity.Ignore(e => e.EmailConfirmed);
            entity.Ignore(e => e.SecurityStamp);
            entity.Ignore(e => e.ConcurrencyStamp);
        });

        modelBuilder.Entity<Posts>(entity =>
        {
            entity.HasKey(p => p.PostId);
            entity.HasOne(p => p.User)
                .WithMany(u => u.Posts)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // User deletion cascades to Posts
            entity.HasOne(p => p.Category)
                .WithMany(c => c.Posts)
                .HasForeignKey(p => p.CategoryId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete on Categories
        });

        modelBuilder.Entity<Comments>(entity =>
        {
            entity.HasKey(c => c.CommentId);
            entity.HasOne(c => c.Post)
                .WithMany(p => p.Comments)
                .HasForeignKey(c => c.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Post deletion cascades to Comments
            entity.HasOne(c => c.User)
                .WithMany(u => u.Comments)
                .HasForeignKey(c => c.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent multiple cascade paths
        });

        modelBuilder.Entity<Points>(entity =>
        {
            entity.HasKey(p => p.PointId);
            entity.HasOne(p => p.User)
                .WithMany(u => u.Points)
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade); // User deletion cascades to Points
        });

        modelBuilder.Entity<Notifications>(entity =>
        {
            entity.HasKey(n => n.NotificationId);

            // Define the relationship with Users
            entity.HasOne(n => n.User)
                .WithMany(u => u.Notifications)
                .HasForeignKey(n => n.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascading paths

            // Define the relationship with Posts
            entity.HasOne(n => n.Post)
                .WithMany(p => p.Notifications)
                .HasForeignKey(n => n.PostId)
                .OnDelete(DeleteBehavior.Restrict); // Use Restrict to avoid cascading paths
        });


        modelBuilder.Entity<Votes>(entity =>
        {
            entity.HasKey(v => v.VoteId);
            entity.HasOne(v => v.User)
                .WithMany(u => u.Votes)
                .HasForeignKey(v => v.UserId)
                .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
            entity.HasOne(v => v.Post)
                .WithMany(p => p.Votes)
                .HasForeignKey(v => v.PostId)
                .OnDelete(DeleteBehavior.Cascade); // Allow Post deletion to cascade to Votes
        });


        modelBuilder.Entity<Categories>(entity =>
        {
            entity.HasKey(c => c.CategoryId);
            entity.HasIndex(c => c.CategoryName).IsUnique();
        });
    }
}
