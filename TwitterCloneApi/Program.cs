using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens; 
using System.Text;
using TwitterCloneApi;
using TwitterCloneApi.Data;
using TwitterCloneApi.Middlewares;
using TwitterCloneApi.Services;
using Npgsql;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using NuGet.Protocol;
using NuGet.Common;
using TwitterCloneApi.Models;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<TokenService>();  
builder.Services.AddControllers(); 


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContextApi>(options =>
{
    //options.UseSqlServer(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));
}
);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["jwt:Key"])),
            ValidateIssuer = false,
            ValidateAudience = false,
            ClockSkew = TimeSpan.Zero
        };
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var token = context.Request.Cookies["access_token"];
                context.Token = token;
                return Task.CompletedTask;
            },
            OnAuthenticationFailed = async context =>
            {
                context.Response.StatusCode = 403; // Set the status code to 403 Forbidden for expired tokens 
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    TokenValidationParameters p = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(builder.Configuration["jwt:Key"])),
                        ValidateIssuer = false,
                        ValidateAudience = false,
                        ClockSkew = TimeSpan.Zero
                    };
                    TokenService t = new TokenService(builder.Configuration);
                    var tokenHandler = new JwtSecurityTokenHandler();
                    string? refreshToken = context.Request.Cookies["refresh_token"];
                    if (refreshToken != null)
                    {
                        JwtValidationResult res = t.ValidateToken(refreshToken);
                        if (res == JwtValidationResult.Valid)
                        {
                            string? nameId = t.DecodeToken(refreshToken).Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;
                            if (nameId == null || nameId == "")
                            {
                                return;
                            }
                            var identity = new ClaimsIdentity(nameId);
                            var principal = new ClaimsPrincipal(identity);
                            ContextApi contextApi = new ContextApi(builder.Services.BuildServiceProvider().GetRequiredService<DbContextOptions<ContextApi>>());

                            UserConfidentials? userConfidentials = await contextApi.UserConfidentials.FindAsync(nameId);
                            if (userConfidentials != null)
                            {
                                if (userConfidentials.RefreshToken == refreshToken)
                                {
                                    string newRefreshToken = t.GenerateRefreshToken(nameId);
                                    string newAccessToken = t.GenerateAccessToken(nameId);

                                    context.Response.Cookies.Append("refresh_token", newRefreshToken);
                                    context.Response.Cookies.Append("access_token", newAccessToken);

                                    userConfidentials.RefreshToken = newRefreshToken;
                                    await contextApi.SaveChangesAsync(); 
                                    context.Principal = principal;
                                    context.Response.StatusCode = 200; // Set the status code to 200 OK if the refresh token is valid
                                    context.Success();
                                }
                            }
                        } 
                    }
                    else
                    {
                        context.Response.StatusCode = 401; // Set the status code to 401 Unauthorized for other authentication failures
                        context.Fail(context.Exception);
                    }
                }
               // return Task.CompletedTask;
            }

         
        };
    });



builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CORS",
                      policy =>
                      {
                          policy.AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod(); 
                      });
}); 

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting(); 

app.UseCors("CORS");

app.UseHttpsRedirection(); 

app.MapControllers();

app.UseAuthentication();

app.UseAuthorization();

app.Run();
 