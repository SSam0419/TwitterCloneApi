using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace TwitterCloneApi.Middlewares
{
     
    public class TokenService
        {
            private const string SecretKey = "your_secret_key_here";

            public static readonly SymmetricSecurityKey SigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(SecretKey));

            public static readonly int AccessTokenExpirationMinutes = 15;

            public static readonly int RefreshTokenExpirationMinutes = 60;

            public static string GenerateAccessToken(string userId)
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

            public static string GenerateRefreshToken(string userId)
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

            public static bool ValidateToken(string token, out SecurityToken validatedToken)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                try
                {
                    tokenHandler.ValidateToken(token, new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = SigningKey,
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    }, out validatedToken);

                    return true;
                }
                catch (Exception)
                {
                    validatedToken = null;
                    return false;
                }
            }

            public static JwtSecurityToken DecodeToken(string token)
            {
                var tokenHandler = new JwtSecurityTokenHandler();

                return tokenHandler.ReadJwtToken(token);
            }
        }
     
}
