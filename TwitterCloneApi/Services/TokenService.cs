using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace TwitterCloneApi.Services
{

    public class TokenService
    {


        public static readonly double AccessTokenExpirationMinutes = 0.15;
        public static readonly int RefreshTokenExpirationMinutes = 60 * 24 * 3;
        public readonly CookieOptions cookieOptions = new CookieOptions
                        {
                            HttpOnly = true,
                            Secure = true, // Set to true if using HTTPS
                            SameSite = SameSiteMode.None, // Set to None if using cross-site requests
                            Expires = DateTimeOffset.UtcNow.AddDays(30)
                        };

        private readonly IConfiguration configuration;
        private readonly SymmetricSecurityKey SigningKey;
        private readonly TokenValidationParameters tokenValidationParameters;
        public TokenService(IConfiguration configuration)
        {
            this.configuration = configuration;
            string key = this.configuration["Jwt:Key"];
            SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key));
            tokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = SigningKey,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
        }

      

        public string GenerateAccessToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(AccessTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public string GenerateRefreshToken(string userId)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, userId)
                }),
                Expires = DateTime.UtcNow.AddMinutes(RefreshTokenExpirationMinutes),
                SigningCredentials = new SigningCredentials(SigningKey, SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public JwtValidationResult ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                tokenHandler.ValidateToken(token, tokenValidationParameters, out var validatedToken);

                if (validatedToken.ValidTo < DateTime.UtcNow)
                {
                    return JwtValidationResult.Expired;
                }

                return JwtValidationResult.Valid;
            }
            catch (SecurityTokenExpiredException)
            {
                return JwtValidationResult.Expired;
            }
            catch (SecurityTokenInvalidIssuerException)
            {
                return JwtValidationResult.InvalidIssuer;
            }
            catch (SecurityTokenInvalidAudienceException)
            {
                return JwtValidationResult.InvalidAudience;
            }
            catch (SecurityTokenInvalidSigningKeyException)
            {
                return JwtValidationResult.InvalidSigningKey;
            }
            catch (Exception)
            {
                return JwtValidationResult.Invalid;
            }
        }

        public JwtSecurityToken DecodeToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var decodedToken = tokenHandler.ReadJwtToken(token);

            return decodedToken;
        }
    }
    public enum JwtValidationResult
    {
        Valid,
        Invalid,
        Expired,
        InvalidIssuer,
        InvalidAudience,
        InvalidSigningKey
    }
}
