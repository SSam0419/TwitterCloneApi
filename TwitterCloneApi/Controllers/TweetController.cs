using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly ContextApi contextApi;
        private readonly TokenService tokenService;

        public TweetController(ContextApi contextApi, TokenService tokenService)
        {
            this.contextApi = contextApi;
            this.tokenService = tokenService;
        }

 
        [HttpGet]
        [Route("GetAllTweetByDate")]
        public  async Task<IActionResult> GetAllTweetByDate()
        {
            List<Tweet> tweets = await contextApi.Tweet
             .Join(contextApi.User,
                 t => t.AuthorId,
                 u => u.Id,
                 (tweet, user) => new { Tweet = tweet, Author = user })
             .OrderByDescending(t => t.Tweet.CreatedAt)
             .Select(t => t.Tweet)
             .ToListAsync();
            return Ok(tweets);
        }
         
        [HttpGet]
        [Route("GetAllTweetByUserId/{id}")]
        public async Task<IActionResult> GetAllTweetByUserId([FromRoute] string id)
        {
            List<Tweet> tweets = await contextApi.Tweet.Where(t => t.Author != null && t.Author.Id == id).ToListAsync();
            return Ok(tweets);
        }


        public class AddTweetBody
        {
            public string tweetContent { get; set; } = "";
        }

        [Authorize]
        [HttpPost]
        [Route("AddTweet")]
        public async Task<IActionResult> AddTweet([FromBody] AddTweetBody addTweetBody)
        { 
            
            Request.Cookies.TryGetValue("access_token", out var cookie); 
            if (cookie != null)
            {
                
                string? authorId = tokenService.DecodeToken(cookie).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                if (await contextApi.User.FindAsync(authorId) == null || authorId == null)
                {
                    return BadRequest("Author Not Found");
                }
                string tweetContent = addTweetBody.tweetContent;
                Tweet newTweet = new Tweet { 
                    TweetId = Guid.NewGuid().ToString(),
                    Content = tweetContent ,
                    AuthorId = authorId,
                    CreatedAt = DateTime.Now.ToUniversalTime(),
                    UpdatedAt = DateTime.Now.ToUniversalTime(),
                    Title = tweetContent,
                };
                try
                {
                    await contextApi.Tweet.AddAsync(newTweet);
             
                    await contextApi.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
             
                return Ok(newTweet);
            }

            return BadRequest("Author Not Found");
        }

        [HttpPut]
        [Route("EditTweet/{id}")]
        public async Task<IActionResult> EditTweet([FromRoute] string id,[FromBody] Tweet tweet)
        {
            Tweet? tweetEdited = await contextApi.Tweet.FindAsync(id);
            if ( tweetEdited != null)
            {
                tweetEdited.Content = tweet.Content;
                tweetEdited.Title = tweet.Title;
                tweetEdited.UpdatedAt = DateTime.Now;
                tweetEdited.Author = tweet.Author;                
                await contextApi.SaveChangesAsync();
                return Ok(tweetEdited);
            }
            return BadRequest();
        }  
         
        [HttpDelete]
        [Route("DeleteTweet/{id}")]
        public async Task<IActionResult> DeleteTweet([FromRoute] string id)
        {
            Tweet? tweetDeleted = await contextApi.Tweet.FindAsync(id);
            if (tweetDeleted != null)
            {
                contextApi.Tweet.Remove(tweetDeleted);
                await contextApi.SaveChangesAsync();
                return Ok(id);
            }
            return BadRequest();
        }
    }
}
