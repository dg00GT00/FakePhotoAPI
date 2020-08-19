using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using FakePhoto.Extensions;
using FakePhoto.Services.ImageSourceService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FakePhoto.Middleware
{
    public class CacheImageMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IImageSourceService _imageSource;
        private readonly ILogger<CacheImageMiddleware> _logger;

        public CacheImageMiddleware(RequestDelegate next, IImageSourceService imageSource,
            ILogger<CacheImageMiddleware> logger)
        {
            _next = next;
            _imageSource = imageSource;
            _logger = logger;
        }

        public async Task Invoke(HttpContext context)
        {
            var (imageName, imagePath) = await GetImageSpecsAsync(context);
            if (File.Exists(imagePath))
            {
                _logger.LogInformation("Returned {ImageName} from cache",
                    _imageSource.ImageNameWithExtension(imageName));
                await _next(context);
            }
        }

        private async Task<Tuple<string, string>> GetImageSpecsAsync(HttpContext context)
        {
            var imageName = context.Request.Path.Value.Split('/').Last();
            var imagePath = await _imageSource.GetImageFullPath(imageName);
            return new Tuple<string, string>(imageName, imagePath);
        }
    }
}