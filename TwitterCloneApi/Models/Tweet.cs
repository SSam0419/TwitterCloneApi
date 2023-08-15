using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
    public class Tweet
    {
        [Key]
        public string TweetId { get; set; } 
        public string Content { get; set; }
        public string Title { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }


        //Foriegn Keys
        public string AuthorId { get; set; }
        [JsonIgnore]
        public User Author { get; set; } = null!;
        [JsonIgnore]
        public ICollection<Comment> Comments { get; set; } = new List<Comment>();
        [JsonIgnore]
        public ICollection<TweetLikes> Likes { get; set; } = new List<TweetLikes>();

  
    }

    public class TweetLikes
    {
        public string UserId { get; set; }
        public string TweetId { get; set; }

        //navigation properties
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Tweet Tweet { get; set; }
    
    }

 
}
