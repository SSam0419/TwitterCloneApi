using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

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
        public User Author { get; set; } = null!;

        public ICollection<Comment> Comments { get; set; } = new List<Comment>();

        public ICollection<User> Likes { get; set; } = new List<User>();

  
    }

 
}
