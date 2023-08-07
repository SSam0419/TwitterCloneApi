using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
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

 
        }
}   
