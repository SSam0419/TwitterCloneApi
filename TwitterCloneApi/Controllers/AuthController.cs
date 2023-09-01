using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Scripting;
using Microsoft.EntityFrameworkCore;
using NuGet.Common;
using System.Data.SqlTypes;
using System.IdentityModel.Tokens.Jwt;
using System.Linq.Expressions;
using System.Net;
using System.Security.Cryptography;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    
    public class AuthController : ControllerBase
    {

        private readonly ContextApi contextApi;
        private readonly TokenService tokenService;

        public AuthController(ContextApi contextApi, TokenService tokenService)
        {
            this.contextApi = contextApi;
            this.tokenService = tokenService;
        }
         
        [Route("sign_in")]
        [HttpPost]
        public async Task<IActionResult> SignIn([FromBody] RegistrationModel registrationModel)
        {
 
            UserConfidentials? user = await contextApi.UserConfidentials.FirstOrDefaultAsync(u => u.Username == registrationModel.username);
            if (user == null)
            {
                return NotFound();
            }
            else
            { 

                bool check = BCrypt.Net.BCrypt.Verify(registrationModel.password, user.Password);

                if (!check)
                {
                    return BadRequest("Incorrec Pasword");
                }

                string accessToken = tokenService.GenerateAccessToken(user.Id);
                string refreshToken = tokenService.GenerateRefreshToken(user.Id); 
 
                Response.Cookies.Append("access_token", accessToken, tokenService.cookieOptions);
                Response.Cookies.Append("refresh_token", refreshToken, tokenService.cookieOptions);
                user.RefreshToken = refreshToken;
                await contextApi.SaveChangesAsync();
                User? signedUser = await contextApi.User.Include(u => u.followings).Include(u => u.followers).Where(u => u.Username == registrationModel.username).FirstOrDefaultAsync();
                return Ok(signedUser);    
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
        {
            UserConfidentials? checkUserConfidential = await contextApi.UserConfidentials.FirstOrDefaultAsync(u => u.Username == registrationModel.username);
            if (checkUserConfidential != null)
            {
                return BadRequest("Username Exist");
            }
            else
            {
                try
                { 
                    string newId = Guid.NewGuid().ToString();
                    User newUser = new User{ Email = registrationModel.username , Bio="",Username = registrationModel.username,IconLink = { },Id = newId,CreatedAt=DateTime.Now.ToUniversalTime(),UpdatedAt=DateTime.Now.ToUniversalTime()}; 
                    UserConfidentials newUserConfidentials = new UserConfidentials{ 
                        Id= newId, 
                        Username=registrationModel.username,
                        RefreshToken="",
                        User=newUser }; 
 

                    string salt = BCrypt.Net.BCrypt.GenerateSalt();
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationModel.password,salt);
                    newUserConfidentials.Password = hashedPassword;
                    newUserConfidentials.Salt = salt;

                    await contextApi.User.AddAsync(newUser);
                    await contextApi.UserConfidentials.AddAsync(newUserConfidentials);
                    await contextApi.SaveChangesAsync();
                    return Ok(newUser);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                    return BadRequest(ex.ToString());
                }
            } 
        }

        public class LogoutBody
        {
            public string userId { get; set; } = "";
        }

        [Route("log_out")]
        [HttpPost]
        public async Task<IActionResult> Logout([FromBody] LogoutBody LogoutBody)
        { 

            Response.Cookies.Append("access_token","",tokenService.cookieOptions);
            Response.Cookies.Append("refresh_token","", tokenService.cookieOptions);

            //delete tokens from database
            UserConfidentials? user = await contextApi.UserConfidentials.FirstOrDefaultAsync(u=> u.Id == LogoutBody.userId);
            if (user == null)
            {
                return BadRequest("Invalid User Id");
            }

            user.RefreshToken = "";
            await contextApi.SaveChangesAsync();

            return Ok();
        }

        [Route("access_token")]
        [HttpGet] 
        public async Task<IActionResult> AccessToken()
        { 
            try
            { 
                string? accessToken = HttpContext.Items["access_token"]?.ToString(); 

                if (accessToken == "" || accessToken == null)
                {
                    return BadRequest("Invalid Token/Missing Token");
                }

                JwtSecurityToken decodeToken = tokenService.DecodeToken(accessToken);
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string? userId = decodeToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                JwtValidationResult tokenResult = tokenService.ValidateToken(accessToken);
                if (tokenResult == JwtValidationResult.Valid)
                {
                    User? profile = await contextApi.User.Include(u => u.followers).Include(u => u.followings).Where(u => u.Id == userId).FirstOrDefaultAsync();
                    if (profile == null)
                    {
                        return BadRequest("Invalid Token");
                    }
                    return Ok(profile);
                }
                else  
                { 
                    return BadRequest("Invalid Token");    
                }
                
            } 
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            } 
            }
 

        public class RegistrationModel
        {
            public string username { get; set; } = "";
            public string password { get; set; } = ""; 
        }

    }
}
