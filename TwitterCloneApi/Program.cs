using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver;
using System.Text;
using TwitterCloneApi;
using TwitterCloneApi.Data;
using TwitterCloneApi.Middlewares;
using TwitterCloneApi.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<TokenService>();  
builder.Services.AddControllers(); 


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<ContextApi>(options =>
{
    MongoClientSettings settings = MongoClientSettings.FromUrl(new MongoUrl(builder.Configuration.GetConnectionString("TwitterCloneApiContext")));
    IMongoClient client = new MongoClient(settings);
    IMongoDatabase database = client.GetDatabase("");

    // Register the database with the DbContext
    options.UseMongoDb(database);


    //options.UseSqlServer(builder.Configuration.GetConnectionString("TwitterCloneApiContext"))
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

app.UseWhen(context => context.Request.Path.ToString().Contains("jwt"),
    builder =>
    {
        builder.UseMiddleware<JwtCookieMiddleware>();
    });

app.Run();
 