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

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<TokenService>();  
builder.Services.AddControllers(); 


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
            OnAuthenticationFailed = context =>
            {
                if (context.Exception.GetType() == typeof(SecurityTokenExpiredException))
                {
                    context.Response.StatusCode = 403; // Set the status code to 403 Forbidden for expired tokens
                }
                return Task.CompletedTask;
            }
        };
    });

builder.Services.AddDbContext<ContextApi>(options =>
{  
    //options.UseSqlServer(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));
}


);

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

app.UseWhen(context => context.Request.Path.ToString().Contains("jwt"),
    builder =>
    {
        builder.UseMiddleware<JwtCookieMiddleware>();
    });

app.Run();
 