using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {

        private readonly ContextApi contextApi;

        public AuthController(ContextApi contextApi)
        {
            this.contextApi = contextApi;
        }
         
        [Route("sign_in")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] UserConfidentials userConfidentials)
        {
            UserConfidentials user = await contextApi.UserConfidentials.FirstOrDefaultAsync(u => u.Username == userConfidentials.Username);
            if (user == null)
            {
                return NotFound();
            }
            else
            {
                if (!BCrypt.Net.BCrypt.Verify(user.Password, userConfidentials.Password))
                {
                    return BadRequest();
                }
                return Ok(await contextApi.User.FirstOrDefaultAsync(u => u.Username == userConfidentials.Username));    
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] UserConfidentials userConfidentials , [FromBody] User user)
        {
            UserConfidentials checkUserConfidential = await contextApi.UserConfidentials.FirstOrDefaultAsync(u => u.Username == userConfidentials.Username);
            if (checkUserConfidential != null)
            {
                return BadRequest("Username Exist");
            }
            else
            {
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(userConfidentials.Password);

                UserConfidentials userConfidentials1 = new UserConfidentials();
                userConfidentials1.Username = userConfidentials.Username;
                userConfidentials1.Password = hashedPassword; 
                await contextApi.User.AddAsync(user);
                await contextApi.UserConfidentials.AddAsync(userConfidentials1);

                return Ok(user);
            }
        }

 
    }
}
