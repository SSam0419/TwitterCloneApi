using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Text;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;

namespace TwitterCloneApi.Middlewares
{

    public class JwtCookieMiddleware
    {
        private readonly RequestDelegate _next;

        public JwtCookieMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IConfiguration _configuration, ContextApi _contextApi, TokenService tokenService)
        {
            string? token = context.Request.Cookies["access_token"];

            if (token == null)
            {
                context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                await context.Response.WriteAsync("Invalid token");
                return;
            }

            if (!string.IsNullOrEmpty(token))
            {
                JwtSecurityToken decodeToken = tokenService.DecodeToken(token);
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                string? userId = decodeToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                try
                {
                    JwtValidationResult tokenResult = tokenService.ValidateToken(token);
                    if (tokenResult == JwtValidationResult.Valid)
                    {
                        User? profile = await _contextApi.User.FindAsync(userId);
                        context.Items.Add("UserProfile", profile);
                        await _next(context);
                    }
                    
                    else if (tokenResult == JwtValidationResult.Expired)
                    {
                        if (userId != null)
                        { 
                            string accessToken = tokenService.GenerateAccessToken(userId);
                            string newRefreshToken = tokenService.GenerateRefreshToken(userId);
                            // Set the new tokens in the response cookies
                            context.Response.Cookies.Append("access_token", accessToken,tokenService.cookieOptions);
                            context.Response.Cookies.Append("refresh_token", newRefreshToken, tokenService.cookieOptions);
                            User? profile = await _contextApi.User.FindAsync(userId);
                            context.Items.Add("UserProfile", profile);
                            await _next(context); 
                        }

                    } 
                    else if (tokenResult == JwtValidationResult.Invalid)
                    {
                        context.Response.StatusCode = (int)HttpStatusCode.BadRequest;
                        await context.Response.WriteAsync("Invalid token");
                        return;
                    }  
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex); 
                }

            }
        
        }
    }
}
