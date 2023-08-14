using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

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

        public UserConfidentials UserConfidentials { get; set; } = null!;
        // Navigation properties
        public ICollection<UserFollowings> Followings { get; set; } = new List<UserFollowings>();
        public ICollection<UserFollowed> Followed { get; set; } = new List<UserFollowed>();
    }

    public class UserFollowings
    {
        public string UserId { get; set; }
        public string FollowingUserId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public User FollowingUser { get; set; }
    }

    public class UserFollowed
    {
        public string UserId { get; set; }
        public string FollowedUserId { get; set; }

        // Navigation properties
        public User User { get; set; }
        public User FollowedUser { get; set; }
    }


    public class UserConfidentials
    {

        [ForeignKey("User")]
        public string Id { get; set; } = "";
        public User User { get; set; } = null!;

        public string Username { get; set; } = "";

        public string Password { get; set; } = "";

        public string? Salt { get; set; } = "";

        public string? RefreshToken { get; set; } 

    }
}
