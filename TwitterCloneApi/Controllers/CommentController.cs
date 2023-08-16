using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
                    CreatedAt = DateTime.Now,
                    Id = Guid.NewGuid().ToString(),
                    UpdatedAt = DateTime.Now,

                };
                await contextApi.Comment.AddAsync(comment);
                await contextApi.SaveChangesAsync();
                return Ok(new { AddCommentBody.TweetId, comment });
            }
            return BadRequest("Invalid author");
        }
         
       
    }
}
