using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using FakePhoto.Extensions;
using FakePhoto.Services.ETagGeneratorService.Interfaces;
using FakePhoto.Services.ImageSourceService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace FakePhoto.Filters
{
    public class ImageCacheFilter : IAsyncActionFilter
    {
        private readonly IImageSourceService _imageSource;
        private readonly ILogger<ImageCacheFilter> _logger;
        private readonly IETagGenerator _eTagGenerator;

        private readonly HttpMethod[] _supportedMethods =
        {
            HttpMethod.Get,
            HttpMethod.Head
        };

        public ImageCacheFilter(IImageSourceService imageSource, ILogger<ImageCacheFilter> logger,
            IETagGenerator eTagGenerator)
        {
            _imageSource = imageSource;
            _logger = logger;
            _eTagGenerator = eTagGenerator;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var ctx = context.HttpContext;
            if (CheckSupportedMethods(ctx))
            {
                var imageName = GetFullImagePath(ctx, out var imagePath);
                if (File.Exists(imagePath))
                {
                    _logger.LogInformation("Returned {ImageName} from cache image file", imageName);
                    var image = await _imageSource.ImageBuilderAsync(imagePath);
                    GenerateCacheHeaders(ctx, image);
                    context.Result = new OkObjectResult(image);
                }
            }
        }

        private async void GenerateCacheHeaders(HttpContext context, string content)
        {
            context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue {Public = true};
            context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("text/html");
            context.Response.GetTypedHeaders().ContentLength = Encoding.ASCII.GetBytes(content).LongLength;
            context.Response.GetTypedHeaders().ETag = await _eTagGenerator.GenerateETag(context);
        }

        private bool CheckSupportedMethods(HttpContext context)
        {
            if (!_supportedMethods.Select(method => method.ToString()).Contains(context.Request.Method))
            {
                throw new MethodUnsupportedException($"{context.Request.Method} is not supported for caching purposes");
            }

            return true;
        }

        private string GetFullImagePath(HttpContext context, out string imagePath)
        {
            var imageName = context.Request.Path.Value.Split('/').Last();
            imagePath = Path.Combine(_imageSource.DirPath, _imageSource.ImageNameWithExtension(imageName));
            return imageName;
        }
    }
}