using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace TwitterCloneApi.Middlewares
{
  

    public static class TokenMiddlewareExtensions
        {
            public static IApplicationBuilder UseTokenMiddleware(this IApplicationBuilder builder)
            {
                return builder.UseMiddleware<TokenMiddleware>();
            }
        }

    public class TokenMiddleware
    {
        private readonly RequestDelegate _next;

        public TokenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            // Check if the request path starts with "/tweet"
            if (!context.Request.Path.StartsWithSegments("/tweet"))
            {
                // Call the next middleware in the pipeline if the request path does not start with "/tweet"
                await _next(context);
                return;
            }

            // Check if the access token is null or expired
            string accessToken = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
            if (string.IsNullOrEmpty(accessToken) || !TokenService.ValidateToken(accessToken, out SecurityToken validatedToken))
            {
                // Get the user ID from the validated refresh token
                string refreshToken = context.Request.Cookies["refresh_token"];
                if (!string.IsNullOrEmpty(refreshToken) && TokenService.ValidateToken(refreshToken, out validatedToken))
                {
                    string userId = validatedToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    // Generate new access and refresh tokens
                    string newAccessToken = TokenService.GenerateAccessToken(userId);
                    string newRefreshToken = TokenService.GenerateRefreshToken(userId);

                    // Set the new tokens in the response headers and cookies
                    context.Response.Headers.Add("Authorization", "Bearer " + newAccessToken);
                    context.Response.Cookies.Append("refresh_token", newRefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Expires = DateTime.UtcNow.AddMinutes(TokenService.RefreshTokenExpirationMinutes)
                    });
                }
                else
                {
                    // Return a 401 Unauthorized response if the refresh token is null or invalid
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    return;
                }
            }

            // Call the next middleware in the pipeline if the access token is valid
            await _next(context);
        }
    }

    public static class TokenMiddlewareConfiguration
    {
        public static void ConfigureTokenMiddleware(this IApplicationBuilder app)
        {
            // Add the middleware that issues new access and refresh tokens
            app.UseMiddleware<TokenMiddleware>();

            // Add the middleware that checks the access and refresh tokens
            app.UseTokenMiddleware();
        }
    }
}
 
