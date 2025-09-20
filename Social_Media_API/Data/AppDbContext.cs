using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Social_Media_API.Model;
using System.Reflection.Emit;

namespace Social_Media_API.Data
{
    public class AppDbContext:IdentityDbContext<User>
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Friendship> Friendships { get; set; }
        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<Like>()
                .Property(l => l.UserId)
                .IsRequired();

            builder.Entity<Like>()
                .Property(l => l.PostId)
                .IsRequired();

            builder.Entity<Like>()
                .HasIndex(l => new { l.PostId, l.UserId })
                .IsUnique();

            builder.Entity<Friendship>()
                .HasIndex(f => new { f.RequesterId, f.ReceiverId })
                .IsUnique();

            builder.Entity<Friendship>()
                .HasCheckConstraint("CK_Friendship_Requester_Receiver", "[RequesterId] <> [ReceiverId]");

            builder.Entity<Friendship>()
                .HasOne(f => f.Requester)    
                .WithMany(u => u.FriendRequestsSent)   
                .HasForeignKey(f => f.RequesterId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Friendship>()
                .HasOne(f => f.Receiver)     
                .WithMany(u => u.FriendRequestsReceived) 
                .HasForeignKey(f => f.ReceiverId)
                .OnDelete(DeleteBehavior.Restrict);
            builder.Entity<Like>()
    .HasOne(l => l.User)
    .WithMany(u => u.Likes)
    .HasForeignKey(l => l.UserId)
    .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Like>()
                .HasOne(l => l.Post)
                .WithMany(p => p.Likes)
                .HasForeignKey(l => l.PostId)
                .OnDelete(DeleteBehavior.Cascade);


            builder.Entity<Post>()
                .HasIndex(p => p.CreatedAt);

            builder.Entity<Comment>()
                .HasIndex(c => c.CreatedAt);
            builder.Entity<Comment>()
    .HasOne(c => c.User)
    .WithMany(u => u.Comments)
    .HasForeignKey(c => c.UserId)
    .OnDelete(DeleteBehavior.Cascade);
            builder.Entity<Comment>()
    .HasOne(c => c.Post)
    .WithMany(p => p.Comments)
    .HasForeignKey(c => c.PostId)
    .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
