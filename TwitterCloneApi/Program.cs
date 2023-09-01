using System.Text.Json.Serialization;
using DotNetEnv;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Middlewares;
using TwitterCloneApi.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

// Add services to the container.
builder.Services.AddScoped<TokenService>();  
builder.Services.AddControllers();
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(); 

builder.Services.AddDbContext<ContextApi>(options =>
{
    //options.UseNpgsql(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));

    options.UseNpgsql(Environment.GetEnvironmentVariable("CONNECTION_NEON"));
}
); 
 

var app = builder.Build(); 

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors(
     policy =>
     {
         policy.WithOrigins("https://twitter-clone-client.pages.dev", "http://localhost:5173")
           .AllowAnyHeader()
           .AllowAnyMethod()
           .AllowCredentials();
     }
    );


app.MapControllers();
 
app.UseAuthentication();

var protectedRoutes = new List<string>
{
    "/api/Auth/access_token", 
};

app.UseWhen(context => protectedRoutes.Contains(context.Request.Path), applicationBuilder =>
{ 
    applicationBuilder.UseMiddleware<JwtCookieMiddleware>();
});

app.UseAuthorization();
app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();
 