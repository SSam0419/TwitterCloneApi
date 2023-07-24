using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ContextApi contextApi;

        public CommentController(ContextApi contextApi)
        {
            this.contextApi = contextApi;
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
         
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value";
        }
         
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }
         
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }
         
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
