using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FakePhoto.Extensions;
using FakePhoto.Services.ImageSourceService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FakePhoto.Middlewares
{
    public class CacheImageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IImageSourceService _imageSourceService;
        private readonly ILogger<CacheImageMiddleware> _logger;

        public CacheImageMiddleware(RequestDelegate next,
            IImageSourceService imageSourceService,
            ILogger<CacheImageMiddleware> logger)
        {
            _next = next;
            _logger = logger;
            _imageSourceService = imageSourceService;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var imageName = GetFullImagePath(context, out var imagePath);
            if (File.Exists(imagePath))
            {
                _logger.LogInformation("Returning cache from {ImageName} image file", imageName);
                var content = await _imageSourceService.ImageBuilderAsync(imagePath);
                context.Response.ContentType = "text/html";
                context.Response.ContentLength = Encoding.ASCII.GetBytes(content).LongLength;
                await context.Response.WriteAsync(content);
            }
            else
            {
                await _next(context);
            }
        }

        private string GetFullImagePath(HttpContext context, out string imagePath)
        {
            var imageName = context.Request.Path.Value.Split('/').Last();
            imagePath = Path.Combine(_imageSourceService.DirPath,
                _imageSourceService.ImageNameWithExtension(imageName));
            return imageName;
        }
    }
}