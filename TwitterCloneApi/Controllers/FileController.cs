using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting.Internal;
using System.IdentityModel.Tokens.Jwt;
using TwitterCloneApi.Data;
using TwitterCloneApi.Models;
using TwitterCloneApi.Services;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TwitterCloneApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [EnableCors("CORS")]
    public class FileController : ControllerBase
    {
        private readonly ContextApi contextApi;
        private readonly TokenService tokenService;
        private readonly IWebHostEnvironment hostingEnvironment;

        public FileController(ContextApi contextApi, TokenService tokenService, IWebHostEnvironment hostingEnvironment)
        {
            this.contextApi = contextApi;
            this.tokenService = tokenService;
            this.hostingEnvironment = hostingEnvironment;
        }


        public class UploadImageBody
        {
            public IFormFile file { get; set; }
        }

        [HttpPost]
        [Route("uploadImage")]
        public IActionResult uploadImage([FromForm] UploadImageBody _file)
        {
            IFormFile file = _file.file; 
            //string? accessToken = HttpContext.Items["access_token"]?.ToString();
            Request.Cookies.TryGetValue("access_token", out var accessToken);

            if (accessToken == "" || accessToken == null)
            {
                return BadRequest("Invalid Token/Missing Token");
            }

            JwtSecurityToken decodeToken = tokenService.DecodeToken(accessToken);
            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            string? userId = decodeToken.Claims.FirstOrDefault(c => c.Type == "nameid")?.Value;

            if (userId == null)
            {
                return BadRequest("userId not found");
            } 


            if (file == null || file.Length <= 0)
            {
                return BadRequest("No file uploaded");
            }

            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploaded");
            string uniqueFileName = userId.ToString();
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                file.CopyTo(stream);
            } 
            return Ok(new { FileName = uniqueFileName });
        }

        [HttpGet]
        [Route("image/{fileName}")]
        public IActionResult Download([FromRoute] string fileName)
        {
            string uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "uploaded");
            string filePath = Path.Combine(uploadsFolder, fileName);

            if (!System.IO.File.Exists(filePath))
            {
                return NotFound();
            }

            var fileStream = new FileStream(filePath, FileMode.Open);
            var mimeType = "application/octet-stream"; // Set the appropriate MIME type based on your file type

            return File(fileStream, mimeType, fileName);
        }
        
    }
}
