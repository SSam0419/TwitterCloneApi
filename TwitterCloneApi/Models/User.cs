using System.Net;

namespace TwitterCloneApi.Models
{
    public class User
    {
        public string Id { get; set; }

        public string Username { get; set; }

        public string Email { get; set; }

        public string? IconLink {   get; set; } 

        public List<User>? Followers { get; set; }




    }

   


    public class UserConfidentials
    {
        public string Username{ get; set; }

        public string Password { get; set; }

        public string? RefreshToken { get; set; }
    }
}
