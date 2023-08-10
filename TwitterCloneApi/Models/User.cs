using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net;

namespace TwitterCloneApi.Models
{
    public class User
    {
        [StringLength(30)]
        public string Id { get; set; } = "";
        [StringLength(30)]
        public string Username { get; set; } = "";
        [StringLength(30)]
        public string Email { get; set; } = "";
        [StringLength(50)]
        public string? IconLink {   get; set; }
     
        public List<User>? Followers { get; set; } 
    }
  public class UserConfidentials
    {
        [Key, ForeignKey("User")]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        [StringLength(30)]
        public string Id { get; set; } = "";
        [StringLength(30)]
        public string Username { get; set; } = "";
        [StringLength(30)]
        public string Password { get; set; } = "";
        [StringLength(30)]
        public string Salt { get; set; } = "";
        [StringLength(30)]
        public string? RefreshToken { get; set; }
        public User? User { get; set; } = new User();
    }
}
