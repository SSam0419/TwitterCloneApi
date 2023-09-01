using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors()]
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
                User? visitUser = await contextApi.User.Include(u=>u.followings).Include(u => u.followers).Where(u=>u.Id == userId).FirstOrDefaultAsync();
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
            UserFollowings? check = await contextApi.UserFollowings.Where(f=>f.FromUserId == from && f.ToUserId == to).FirstOrDefaultAsync();
            if (check != null)
            {
                contextApi.UserFollowings.Remove(check);
            }
            else
            {
                await contextApi.UserFollowings.AddAsync(u);
            }
            await contextApi.SaveChangesAsync();
            return Ok();
        }

        public class UpdateProfileBody
        {
            public string bio { get; set; } = "";
            public string userId { get; set; } = "";
        }

        [HttpPost]
        [Route("UpdateProfile")]
        public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileBody UpdateProfileBody)
        {
            User? user = await contextApi.User.FindAsync(UpdateProfileBody.userId);
            if (user == null)
            {
                return NotFound();
            }
            user.Bio = UpdateProfileBody.bio;
            user.UpdatedAt = DateTime.Now.ToUniversalTime();    
            await contextApi.SaveChangesAsync();
            return Ok();
        }
 
        
    }
}
