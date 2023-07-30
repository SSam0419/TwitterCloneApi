using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using static System.Net.Mime.MediaTypeNames;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TweetController : ControllerBase
    {
        private readonly ContextApi contextApi;

        public TweetController(ContextApi contextApi)
        {
            this.contextApi = contextApi;
        }
         
        [HttpGet]
        [Route("jwt/GetAllTweetByDate")]
        public  async Task<IActionResult> GetAllTweetByDate()
        {   
            List<Tweet> tweets = await contextApi.Tweet.Include(t => t.Author).OrderByDescending(t => t.CreatedAt).ToListAsync();
            return Ok(tweets);
        }
         
        [HttpGet]
        [Route("GetAllTweetByUserId/{id}")]
        public async Task<IActionResult> GetAllTweetByUserId([FromRoute] string id)
        {
            List<Tweet> tweets = await contextApi.Tweet.Where(t => t.Author != null && t.Author.Id == id).ToListAsync();
            return Ok(tweets);
        }

        [HttpPost]
        [Route("AddTweet")]
        public async Task<IActionResult> AddTweet([FromBody] Tweet tweet)
        {
            tweet.TweetId = Guid.NewGuid().ToString();


            //to be finished : implement auth controllers
            User dummy = new User();
            dummy.Username = "test";
            dummy.Email = "test";
            dummy.Id = Guid.NewGuid().ToString();
            tweet.Author = dummy;
            if (await contextApi.Tweet.FindAsync("test") != null)
            {
                await contextApi.User.AddAsync(dummy);
            }
            //end

            await contextApi.Tweet.AddAsync(tweet);
             
            await contextApi.SaveChangesAsync();
             
            return Ok(tweet);
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
