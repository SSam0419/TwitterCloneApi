using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {

        private readonly ContextApi contextApi;

        public UserController(ContextApi contextApi)
        {
            this.contextApi = contextApi;
        }


        public class VisitProfileBody
        {
            public string userId { get; set; } = ""; 
        }

        [HttpPost]
        [Route("VisitProfile")]
        public async Task<IActionResult> VisitProfile([FromBody] VisitProfileBody VisitProfileBody)
        {
            string userId = VisitProfileBody.userId;
            if (userId != null)
            {
                User? visitUser = await contextApi.User.FindAsync(userId);
                if (visitUser != null)
                {
                    return Ok(visitUser);
                }
            }
            return NotFound("Invaid User");
        }


        public class FollowUserBody
        {
            public string From { get; set; } = "";
            public string To { get; set; } = "";
        }

        //follow user
        [HttpPost] 
        [Route("follow")]
        public async Task<IActionResult> FollowUser([FromBody] FollowUserBody FollowUserBody)
        {
            string from = FollowUserBody.From;
            string to = FollowUserBody.To;
            User? fromUser = await contextApi.User.FindAsync(from);
            User? toUser = await contextApi.User.FindAsync(to);

            if ( fromUser == null || toUser == null)
            {
                return BadRequest("User cannot be found");
            }

            UserFollowings u = new UserFollowings { ToUserId = to, FromUserId = from };
            UserFollowings? check = await contextApi.UserFollowings.FindAsync(u);
            if (check != null)
            {
                return BadRequest("Alread followed");
            }
            await contextApi.UserFollowings.AddAsync(u);
            await contextApi.SaveChangesAsync();
            return Ok();
        }

        //unfollower user
        [HttpPost] 
        [Route("Unfollow")]
        public async Task<IActionResult> UnfollowUser([FromBody] FollowUserBody FollowUserBody)
        {
            string from = FollowUserBody.From;
            string to = FollowUserBody.To;
            User? fromUser = await contextApi.User.FindAsync(from);
            User? toUser = await contextApi.User.FindAsync(to);

            if (fromUser == null || toUser == null)
            {
                return BadRequest("User cannot be found");
            }

            UserFollowings u = new UserFollowings { ToUserId = to, FromUserId = from };
            UserFollowings? deletingU = await contextApi.UserFollowings.FindAsync(u);
            if(deletingU == null)
            {
                return BadRequest("User cannot be found");
            }
            contextApi.UserFollowings.Remove(deletingU);
            await contextApi.SaveChangesAsync();
            return Ok();
        }
        
    }
}
