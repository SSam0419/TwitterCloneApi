using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;
using System.Text.Json.Serialization;

namespace TwitterCloneApi.Models
{
    public class User
    {
        [Key]
        public string Id { get; set; } = "";
        public string Username { get; set; } = "";
        public string Email { get; set; } = ""; 
        public string? IconLink {   get; set; }
        //foreign key          
        [JsonIgnore]
        public UserConfidentials UserConfidentials { get; set; } = null!;
        [JsonIgnore]
        // Navigation properties
        public ICollection<Tweet> Tweet { get; set; } = new List<Tweet>();
        [JsonIgnore] 
        public ICollection<Comment> Comment { get; set; } = new List<Comment>();
        [JsonIgnore] 
        public ICollection<UserFollowings> ToFollowings { get; set; } = new List<UserFollowings>();
        [JsonIgnore]
        public ICollection<UserFollowings> FromFollowings { get; set; } = new List<UserFollowings>();
        [JsonIgnore]
        public ICollection<TweetLikes> TweetLikes { get; set; } = new List<TweetLikes>(); 
        [JsonIgnore]
        public ICollection<CommentLikes> CommentLikes { get; set; } = new List<CommentLikes>();

    }

    public class UserFollowings
    {
        public string FromUserId { get; set; }
        public string ToUserId { get; set; }

        // Navigation properties
        [JsonIgnore]
        public User FromUser { get; set; }
        [JsonIgnore]
        public User ToUser { get; set; } 
    }
     
    public class UserConfidentials
    {

        [ForeignKey("User")]
        public string Id { get; set; } = "";

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string? Salt { get; set; } = "";

        public string? RefreshToken { get; set; }

        //navigation propeties
        [JsonIgnore]
        public User User { get; set; } = null!;

    }
}
