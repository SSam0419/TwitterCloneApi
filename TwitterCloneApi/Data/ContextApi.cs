
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Data
{
    public class ContextApi : DbContext
    {
        public ContextApi(DbContextOptions options) : base(options)
        {
        }

        public DbSet<Tweet> Tweet { get; set; }

        public DbSet<User> User { get; set; }

        public DbSet<Comment> Comment { get; set; }

        public DbSet<UserConfidentials> UserConfidentials { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //user
            modelBuilder.Entity<UserConfidentials>()
                .HasOne<User>(uc => uc.User)
                .WithOne(u => u.UserConfidentials)
                .HasForeignKey<UserConfidentials>(uc => uc.Id);

            modelBuilder.Entity<UserFollowings>()
                .HasKey(uf => new { uf.UserId, uf.FollowingUserId });

            modelBuilder.Entity<UserFollowed>()
                .HasKey(uf => new { uf.UserId, uf.FollowedUserId });

            modelBuilder.Entity<UserFollowings>()
               .HasOne(uf => uf.User)
               .WithMany(u => u.Followings)
               .HasForeignKey(uf => uf.UserId)
               .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollowings>()
                .HasOne(uf => uf.FollowingUser)
                .WithMany()
                .HasForeignKey(uf => uf.FollowingUserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollowed>()
                .HasOne(uf => uf.User)
                .WithMany(u => u.Followed)
                .HasForeignKey(uf => uf.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<UserFollowed>()
                .HasOne(uf => uf.FollowedUser)
                .WithMany()
                .HasForeignKey(uf => uf.FollowedUserId)
                .OnDelete(DeleteBehavior.Restrict);



            //comment
            modelBuilder.Entity<Comment>()
                .HasMany<User>(c => c.Likes)
                .WithMany();            
            modelBuilder.Entity<User>()
                .HasMany<Comment>()
                .WithOne(c=>c.User).HasForeignKey(c => c.AuthorId);



            //tweet 
            modelBuilder.Entity<Tweet>()
                .HasOne<User>(t => t.Author)
                .WithMany()
                .HasForeignKey(t => t.AuthorId)
                .IsRequired();

            modelBuilder.Entity<Tweet>()
                .HasMany<Comment>(t => t.Comments)
                .WithOne(c => c.Tweet)
                .HasForeignKey(c => c.TweetId);

            modelBuilder.Entity<Tweet>().HasMany<User>(t => t.Likes)
                .WithMany();

        }
    }
}   
