
using Microsoft.Extensions.Primitives;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
    public class Comment
    {
        [Key]
        public string Id { get; set; } = "";
        public string Content { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        //Foreign Key 
        public string AuthorId { get; set; }
        public string TweetId { get; set; }
        [JsonIgnore]
        public User User { get; set; }
        [JsonIgnore]
        public Tweet Tweet { get; set; }
        [JsonIgnore]
        public ICollection<User> Likes { get; set; } = new List<User>();
    } 
}
