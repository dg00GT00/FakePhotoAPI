using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakePhoto.Extensions;
using FakePhoto.Services.ImageSourceService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;

namespace FakePhoto.Filters
{
    public class ImageCacheFilter : IAsyncActionFilter
    {
        private readonly IImageSourceService _imageSourceService;
        private readonly ILogger<ImageCacheFilter> _logger;

        private readonly HttpMethod[] _supportedMethods =
        {
            HttpMethod.Get,
            HttpMethod.Head
        };

        public ImageCacheFilter(IImageSourceService imageSourceService, ILogger<ImageCacheFilter> logger)
        {
            _imageSourceService = imageSourceService;
            _logger = logger;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ctx = context.HttpContext;
            var imageName = GetFullImagePath(ctx, out var imagePath);
            if (File.Exists(imagePath))
            {
                _logger.LogInformation("Returning cache from {ImageName} image file", imageName);
                var content = await _imageSourceService.ImageBuilderAsync(imagePath);
                ctx.Response.ContentType = "text/html";
                ctx.Response.ContentLength = Encoding.ASCII.GetBytes(content).LongLength;
                await ctx.Response.WriteAsync(content);
            }
        }

        private bool CheckSupportedMethods(HttpContext context)
        {
            if (_supportedMethods.Select(method => method.ToString()).Contains(context.Request.Method))
            {
                return true;
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