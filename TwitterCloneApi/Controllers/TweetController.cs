using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Xml.Linq;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;

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
            try
            { 
                List<Tweet> result = await contextApi.Tweet
                    .Include(t => t.Author)  
                    .Include(t => t.Likes)
                    .Include(t=>t.TweetBookmarks)
                    .Include(t => t.Comments).ThenInclude(c => c.Author).ThenInclude(c => c.CommentLikes)
                    .ToListAsync();
                return Ok(result);
            }
            catch (Exception e)
            {
                Console.WriteLine();
            }
            return BadRequest();
        }

        [HttpGet]
        [Route("GetAllTweetByUserId/{id}")]
        public async Task<IActionResult> GetAllTweetByUserId([FromRoute] string id)
        {
            List<Tweet> tweets = await contextApi.Tweet
                    .Include(t => t.Author)
                    .Include(t => t.Likes)
                    .Include(t => t.TweetBookmarks)
                    .Include(t => t.Comments)
                    .ThenInclude(c => c.Author).ThenInclude(c => c.CommentLikes)
                    .Where(t => t.Author != null && t.Author.Id == id).ToListAsync();
            return Ok(tweets);
        }


        public class AddTweetBody
        {
            public string tweetContent { get; set; } = "";
        }
         
        [HttpPost]
        [Route("AddTweet")]
        public async Task<IActionResult> AddTweet([FromBody] AddTweetBody addTweetBody)
        { 
            
            Request.Cookies.TryGetValue("access_token", out var cookie); 
            if (cookie != null)
            {
                
                string? authorId = tokenService.DecodeToken(cookie).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                User? author = await contextApi.User.FindAsync(authorId);
                if (author  == null || authorId == null)
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
                    Author = author
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
             
                return Ok(new {newTweet, author});
            }

            return BadRequest("Author Not Found");
        }


        public class EditTweetBody
        {
            public string tweetContent { get; set; } = "";
        }
        [HttpPut]
        [Route("EditTweet/{id}")]
        public async Task<IActionResult> EditTweet([FromRoute] string id,[FromBody] EditTweetBody EditTweetBody)
        {
            try
            {
                Tweet? tweetEdited = await contextApi.Tweet
                        .Include(t => t.Author)
                        .Include(t => t.Likes)
                        .Include(t => t.TweetBookmarks)
                        .Include(t => t.Comments)
                        .ThenInclude(c => c.Author).ThenInclude(c => c.CommentLikes).Where(t=>t.TweetId == id).FirstOrDefaultAsync();
                if ( tweetEdited != null)
                {
                    tweetEdited.Content = EditTweetBody.tweetContent;
                    tweetEdited.Title = EditTweetBody.tweetContent;
                    tweetEdited.UpdatedAt = DateTime.Now.ToUniversalTime();              
                    await contextApi.SaveChangesAsync();
                    return Ok(tweetEdited);
                }
                return NotFound();
            }
            catch  (Exception ex)
            {
                return BadRequest(ex.Message);
            }
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


        public class BookmarkTweetBody
        {
            public string TweetId { get; set; } = "";
            public string UserId { get; set; } = ""; 

        }
        [HttpPost]
        [Route("BookmarkTweet")]
        public async Task<IActionResult> BookmarkTweet([FromBody] BookmarkTweetBody BookmarkTweetBody)
        {
            try
            {
                TweetBookmarks? check = await contextApi.TweetBookmarks.Where((tb) => tb.TweetId == BookmarkTweetBody.TweetId && tb.UserId== BookmarkTweetBody.UserId).FirstOrDefaultAsync();
                if (check == null)
                {
                    await contextApi.TweetBookmarks.AddAsync(new TweetBookmarks { TweetId = BookmarkTweetBody.TweetId, UserId = BookmarkTweetBody.UserId }) ;
                }
                else
                {
                    contextApi.TweetBookmarks.Remove(check);
                }
                await contextApi.SaveChangesAsync();    
                return Ok();
            }
            catch (Exception ex) 
            {
                return BadRequest(ex.Message);  
            }
        }

        [HttpGet]
        [Route("GetBookmarkedTweet/{id}")]
        public async Task<IActionResult> GetBookmarkedTweet([FromRoute] string id)
        {
            try
            {
                List<Tweet> result = await contextApi.Tweet
                 .Include(t => t.Author)
                 .Include(t => t.Likes)
                 .Include(t => t.TweetBookmarks)
                 .Include(t => t.Comments).ThenInclude(c => c.Author).ThenInclude(c => c.CommentLikes)
                 .Where(t => t.TweetBookmarks.Any(tb => tb.UserId == id))
                 .ToListAsync();
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        public class LikeTweetBody
        {
            public string TweetId { get; set; }
        }

        [HttpPost]
        [Route("LikeTweet")]
        public async Task<IActionResult> LikeTweet([FromBody] LikeTweetBody LikeTweetBody)
        {
            string TweetId = LikeTweetBody.TweetId;
            try
            {
                Request.Cookies.TryGetValue("access_token", out var cookie);
                if (cookie != null)
                {

                    string? authorId = tokenService.DecodeToken(cookie).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

                    if (await contextApi.User.FindAsync(authorId) == null || authorId == null)
                    {
                        return NotFound("Author Not Found");
                    }

                    TweetLikes tweetLikes = new TweetLikes {TweetId = TweetId, UserId = authorId };
                    TweetLikes? check = await contextApi.TweetLikes.FirstOrDefaultAsync(tl => tl.TweetId == TweetId && tl.UserId == authorId);
                    if (check == null) 
                    { 
                        await contextApi.TweetLikes.AddAsync(tweetLikes);
                        await contextApi.SaveChangesAsync();
                        return Ok(new { TweetId, authorId , Action = "Like"});
                    }
                    else
                    {
                        contextApi.TweetLikes.Remove(check);
                        await contextApi.SaveChangesAsync();
                        return Ok(new { TweetId, authorId , Action = "Unlike" });
                    }
                }
                return BadRequest();            
            }
            catch (Exception ex) {
                return BadRequest(ex.Message);
            }
        }



         
    
    
    }
}
