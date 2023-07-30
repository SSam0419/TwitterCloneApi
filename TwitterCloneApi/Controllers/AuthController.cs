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
        public async Task<IActionResult> SignIn([FromBody] UserConfidentials userConfidentials)
        {
            UserConfidentials? user = await contextApi.UserConfidentials.FirstOrDefaultAsync(u => u.Username == userConfidentials.Username);
            if (user == null)
            {
                return NotFound();
            }
            else
            {

                string hashedEnteredPassword = BCrypt.Net.BCrypt.HashPassword(user.Password, user.Salt);


                if (!BCrypt.Net.BCrypt.Verify(user.Password, hashedEnteredPassword))
                {
                    return BadRequest("Incorrec Pasword");
                }

                string accessToken = tokenService.GenerateAccessToken(user.Id);
                string refreshToken = tokenService.GenerateRefreshToken(user.Id); 
 
                Response.Cookies.Append("access_token", accessToken, tokenService.cookieOptions);
                Response.Cookies.Append("refresh_token", refreshToken, tokenService.cookieOptions);

                return Ok(await contextApi.User.FirstOrDefaultAsync(u => u.Username == userConfidentials.Username));    
            }
        }

        [Route("register")]
        [HttpPost]
        public async Task<IActionResult> Register([FromBody] RegistrationModel registrationModel)
        {
            UserConfidentials? checkUserConfidential = await contextApi.UserConfidentials.FirstOrDefaultAsync(u => u.Username == registrationModel.UserConfidentials.Username);
            if (checkUserConfidential != null)
            {
                return BadRequest("Username Exist");
            }
            else
            {
                User newUser = registrationModel.User;
                UserConfidentials newUserConfidentials = registrationModel.UserConfidentials;
                string newId = Guid.NewGuid().ToString();
                newUser.Id = newId;
                newUserConfidentials.Id = newId;
                newUserConfidentials.User = newUser;

                string salt = BCrypt.Net.BCrypt.GenerateSalt(1);
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(registrationModel.UserConfidentials.Password, salt);
                newUserConfidentials.Password = hashedPassword;
                newUserConfidentials.Salt = salt;

                await contextApi.User.AddAsync(newUser);
                await contextApi.UserConfidentials.AddAsync(newUserConfidentials);
                await contextApi.SaveChangesAsync();
                return Ok(newUser);
            } 
        }

        [Route("access_token")]
        [HttpPost]
        public async Task<IActionResult> AccessToken([FromBody] string accessToken)
        {
            JwtSecurityToken decodeToken = tokenService.DecodeToken(accessToken);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string? userId = decodeToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
            try
            {
                JwtValidationResult tokenResult = tokenService.ValidateToken(accessToken);
                if (tokenResult == JwtValidationResult.Valid)
                {
                    User? profile = await contextApi.User.FindAsync(userId);
                    return Ok(profile);
                }

                else if (tokenResult == JwtValidationResult.Expired)
                {
                    if (userId != null)
                    {
                        string newAccessToken = tokenService.GenerateAccessToken(userId);
                        string newRefreshToken = tokenService.GenerateRefreshToken(userId);

                        HttpContext.Response.Cookies.Append("access_token", accessToken, tokenService.cookieOptions);
                        HttpContext.Response.Cookies.Append("refresh_token", newRefreshToken, tokenService.cookieOptions);
                        User? profile = await contextApi.User.FindAsync(userId);
                        return Ok(profile);
                    }
                    return BadRequest("Token decoded does not have valid user id.");

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
            return BadRequest();
            }
 

        public class RegistrationModel
        {
            public User User { get; set; }  = new User();
            public UserConfidentials UserConfidentials { get; set; } = new UserConfidentials();
        }

    }
}
