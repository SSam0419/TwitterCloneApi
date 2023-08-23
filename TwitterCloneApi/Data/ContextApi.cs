
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
        public DbSet<TweetLikes> TweetLikes { get; set; }
        public DbSet<TweetBookmarks> TweetBookmarks { get; set; }

        public DbSet<User> User { get; set; }
        public DbSet<UserFollowings> UserFollowings { get; set; } 
        public DbSet<Comment> Comment { get; set; }
        public DbSet<CommentLikes> CommentLikes { get; set; }

        public DbSet<UserConfidentials> UserConfidentials { get; set;}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //user
            modelBuilder.Entity<UserConfidentials>()
                .HasOne<User>(uc => uc.User)
                .WithOne(u => u.UserConfidentials)
                .HasForeignKey<UserConfidentials>(uc => uc.Id);
            modelBuilder.Entity<UserFollowings>()
                .HasKey(uf => new { uf.FromUserId, uf.ToUserId });
            modelBuilder.Entity<UserFollowings>()
                .HasOne(uf => uf.FromUser)
                .WithMany(u => u.followings)
                .HasForeignKey(uf => uf.FromUserId)
                .IsRequired();
            modelBuilder.Entity<UserFollowings>()
                .HasOne(uf => uf.ToUser)
                .WithMany(u => u.followers)
                .HasForeignKey(uf => uf.ToUserId)
                .IsRequired();

            //comment       
            modelBuilder.Entity<User>()
                .HasMany<Comment>(u=>u.Comment)
                .WithOne(c=>c.Author).HasForeignKey(c => c.AuthorId);
            //comment likes
            modelBuilder.Entity<CommentLikes>()
                .HasKey(cl => new { cl.UserId, cl.CommentId });
            modelBuilder.Entity<CommentLikes>()
                .HasOne(cl => cl.user)
                .WithMany(u => u.CommentLikes)
                .HasForeignKey(cl => cl.UserId);
            modelBuilder.Entity<CommentLikes>()
                .HasOne(cl => cl.Comment)
                .WithMany(c => c.Likes)
                .HasForeignKey(cl => cl.CommentId);

            //tweet 
            modelBuilder.Entity<Tweet>()
                .HasOne<User>(t => t.Author)
                .WithMany(u=>u.Tweet)
                .HasForeignKey(t => t.AuthorId)
                .IsRequired();
            //tweet comment
            modelBuilder.Entity<Tweet>()
                .HasMany<Comment>(t => t.Comments)
                .WithOne(c => c.Tweet)
                .HasForeignKey(c => c.TweetId);

            //tweet likes
            modelBuilder.Entity<TweetLikes>()
                .HasKey(tl => new { tl.UserId, tl.TweetId });
            modelBuilder.Entity<TweetLikes>()
                .HasOne(tl => tl.User)
                .WithMany(u => u.TweetLikes)
                .HasForeignKey(tl => tl.UserId);
            modelBuilder.Entity<TweetLikes>()
                .HasOne(tl => tl.Tweet)
                .WithMany(t => t.Likes)
                .HasForeignKey(tl => tl.TweetId);
            //tweet bookmark
            modelBuilder.Entity<TweetBookmarks>()
                .HasKey(tb => new {tb.UserId, tb.TweetId });
            modelBuilder.Entity<TweetBookmarks>()
                .HasOne(tb => tb.Tweet)
                .WithMany(t => t.TweetBookmarks)
                .HasForeignKey(tb => tb.TweetId);
            modelBuilder.Entity<TweetBookmarks>()
                .HasOne(tb => tb.User)
                .WithMany(u => u.TweetBookmarks)
                .HasForeignKey(tb => tb.UserId);


        }
    }
}   
