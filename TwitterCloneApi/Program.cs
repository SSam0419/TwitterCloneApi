using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;
using TwitterCloneApi.Data;
using TwitterCloneApi.Middlewares;
using TwitterCloneApi.Services;

var builder = WebApplication.CreateBuilder(args);

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
    //options.UseSqlServer(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));
    options.UseNpgsql(builder.Configuration.GetConnectionString("TwitterCloneApiContext"));
}
);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "CORS",
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:5173")
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials();
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

var protectedRoutes = new List<string>
{
    "/api/Auth/access_token", 
};
app.UseWhen(context => protectedRoutes.Contains(context.Request.Path), applicationBuilder =>
{ 
    applicationBuilder.UseMiddleware<JwtCookieMiddleware>();
});


app.UseHttpsRedirection(); 

app.MapControllers();
 
app.UseAuthentication();

app.UseAuthorization();

app.Run();
 