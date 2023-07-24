namespace TwitterCloneApi.Models
{
    public class Comment
    {
        public string Id { get; set; }
        public string Content { get; set; }
        public User Author { get; set; }

        public List<User> Likes { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }

        public string TweetId { get; set; }
        public Tweet Tweet { get; set; }
    }
}
