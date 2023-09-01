using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;
using static TwitterCloneApi.Controllers.TweetController;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]
    public class CommentController : ControllerBase
    {
        private readonly ContextApi contextApi;
        private readonly TokenService tokenService;

        public CommentController(ContextApi contextApi ,TokenService tokenService)
        {
            this.contextApi = contextApi;
            this.tokenService = tokenService;
        }

        [HttpGet]
        [Route("GetCommentById/{id}")]
        public async Task<IActionResult> GetCommentById([FromRoute] string id)
        {
            List<Comment> comments = await contextApi.Comment.Where(c => c.TweetId == id).ToListAsync();

            if (comments == null)
            {
                return NotFound();
            }
             

            return Ok(comments); 
        }


        public class AddCommentBody
        {
            public string TweetId { get; set; }
            public string Content { get; set; }
        }
        [HttpPost]
        [Route("AddComment")]
        public async Task<IActionResult> AddComment([FromBody] AddCommentBody AddCommentBody)
        {
            try
            {
                Request.Cookies.TryGetValue("access_token", out var cookie);
                if (cookie != null)
                {

                    string? authorId = tokenService.DecodeToken(cookie).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                    if (authorId == null)
                    {
                        return BadRequest("Invalid Author");
                    }
                    Comment comment = new Comment {
                        AuthorId = authorId,
                        TweetId = AddCommentBody.TweetId,
                        Content = AddCommentBody.Content,
                        Id = Guid.NewGuid().ToString(),
                        CreatedAt = DateTime.Now.ToUniversalTime(),
                        UpdatedAt = DateTime.Now.ToUniversalTime(),
                    };
                    await contextApi.Comment.AddAsync(comment);
                    await contextApi.SaveChangesAsync();    
                    return Ok(new { AddCommentBody.TweetId, comment });
            }
            }
            catch (Exception ex)
            {
               Console.WriteLine(ex.ToString());
            }
            return BadRequest("Invalid author");
        }


        public class LikeCommentBody
        {
            public string CommentId { get; set; }
        }

        [HttpPost]
        [Route("LikeComment")]
        public async Task<IActionResult> LikeComment([FromBody] LikeCommentBody LikeCommentBody)
        { 
            string CommentId = LikeCommentBody.CommentId;
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

                    CommentLikes commentLike = new CommentLikes { CommentId = CommentId, UserId = authorId };
                    CommentLikes? check = await contextApi.CommentLikes.FirstOrDefaultAsync(tl => tl.CommentId == CommentId && tl.UserId == authorId);
                    if (check == null)
                    {
                        await contextApi.CommentLikes.AddAsync(commentLike);
                        await contextApi.SaveChangesAsync();
                        return Ok(new { CommentId, authorId, Action = "Like" });
                    }
                    else
                    {
                        contextApi.CommentLikes.Remove(check);
                        await contextApi.SaveChangesAsync();
                        return Ok(new { CommentId, authorId, Action = "Unlike" });
                    }
                }
                return BadRequest();
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}
