using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace TwitterCloneApi.Models
{
    public class Tweet
    {
        public string  TweetId { get; set;}
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public User? Author { get; set; }

        public List<Comment>? Comment { get; set; }

        public List<User>? Likes { get; set; }

  
    }
}
