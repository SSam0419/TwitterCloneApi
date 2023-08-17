﻿using Microsoft.AspNetCore.Authorization;
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
            var query = from tweet in contextApi.Tweet
                        join user in contextApi.User on tweet.AuthorId equals user.Id into userGroup
                        from user in userGroup
                        join comments in contextApi.Comment on tweet.TweetId equals comments.TweetId into commentGroup
                        from comments in commentGroup.DefaultIfEmpty()
                        join tweetLikes in contextApi.TweetLikes on tweet.TweetId equals tweetLikes.TweetId into tweetLikesGroup
                        from tweetLikes in tweetLikesGroup.DefaultIfEmpty()
                        group new { comments, tweetLikes } by new
                        {
                            tweet.TweetId,
                            tweet.Content,
                            tweet.AuthorId,
                            tweet.CreatedAt,
                            tweet.UpdatedAt,
                            tweet.Title,
                            Author = new
                            {
                                user.Email,
                                user.IconLink,
                                user.Id,
                                user.Username,
                            },
                        } into tweetGroup
                        orderby tweetGroup.Key.CreatedAt descending
                        select new
                        {
                            tweetGroup.Key.TweetId,
                            tweetGroup.Key.Content,
                            tweetGroup.Key.AuthorId,
                            tweetGroup.Key.CreatedAt,
                            tweetGroup.Key.UpdatedAt,
                            tweetGroup.Key.Title,
                            tweetGroup.Key.Author,
                            likes = tweetGroup.Select(x => x.tweetLikes.UserId).Where(x => x != null).Distinct().ToList(),
                            Comments = tweetGroup.Select(x => x.comments).Where(x => x != null).Distinct().ToList()
                        };

            var result = await query.ToListAsync();
            return Ok(result);
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






        //[HttpPost]
        //[Route("UnlikeTweet")]
        //public async Task<IActionResult> UnlikeTweet([FromBody] LikeTweetBody UnikeTweetBody)
        //{
        //    string TweetId = UnikeTweetBody.TweetId;
        //    try
        //    {
        //        Request.Cookies.TryGetValue("access_token", out var cookie);
        //        if (cookie != null)
        //        {

        //            string? authorId = tokenService.DecodeToken(cookie).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

        //            if (await contextApi.User.FindAsync(authorId) == null || authorId == null)
        //            {
        //                return BadRequest("Author Not Found");
        //            }

        //            TweetLikes tweetLikes = new TweetLikes { TweetId = TweetId, UserId = authorId };
        //            contextApi.TweetLikes.Remove(tweetLikes);
        //            await contextApi.SaveChangesAsync();
        //            return Ok(new {TweetId, authorId });
        //        }
        //        return BadRequest();
        //    }
        //    catch (Exception ex)
        //    {
        //        return BadRequest(ex.Message);
        //    }
        //}
    
    
    }
}
