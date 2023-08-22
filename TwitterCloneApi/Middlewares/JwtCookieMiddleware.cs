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

            if (string.IsNullOrEmpty(token))
            {
                context.Response.StatusCode = 404; 
                await context.Response.WriteAsync("Invalid token"); 
                return ;
            }

            if (!string.IsNullOrEmpty(token))
            {
                try
                {
                    JwtSecurityToken decodeToken = tokenService.DecodeToken(token);
                    JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                    string? userId = decodeToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                    JwtValidationResult tokenResult = tokenService.ValidateToken(token);
                    if (tokenResult == JwtValidationResult.Valid)
                    {
                        context.Items["access_token"] = token;
                        await _next(context);
                    }
                    
                    else if (tokenResult == JwtValidationResult.Expired)
                    {
                        if (userId != null)
                        { 
                            string newAccessToken = tokenService.GenerateAccessToken(userId);
                            string newRefreshToken = tokenService.GenerateRefreshToken(userId);
                            // Set the new tokens in the response cookies
                            context.Response.Cookies.Append("access_token", newAccessToken, tokenService.cookieOptions);
                            context.Response.Cookies.Append("refresh_token", newRefreshToken, tokenService.cookieOptions);
                            context.Items["access_token"] = newAccessToken;
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
