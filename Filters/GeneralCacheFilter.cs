using System;
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
using Microsoft.AspNetCore.ResponseCaching;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;

namespace FakePhoto.Filters
{
    public class GeneralCacheFilter : IAsyncActionFilter
    {
        private readonly IImageSourceService _imageSource;
        private readonly ILogger<GeneralCacheFilter> _logger;
        private readonly IETagGenerator _eTagGenerator;
        private readonly HttpMethod[] _supportedMethods = {HttpMethod.Get, HttpMethod.Head};
        public string[] VaryByQueryKeys { get; set; } = {"width", "height"};
        public int MaxAge { get; set; } = 10;

        public GeneralCacheFilter(IImageSourceService imageSource, ILogger<GeneralCacheFilter> logger,
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
                try
                {
                    var (imageName, imagePath) = await GetImageSpecsAsync(ctx);
                    _logger.LogInformation("Returned {ImageName} from cache",
                        _imageSource.ImageNameWithExtension(imageName));
                    var image = _imageSource.GenerateImageTag(imagePath);
                    // await GenerateETagHeaderAsync(ctx);
                    context.Result = new ContentResult {Content = image, ContentType = "text/html"};
                }
                catch (FileNotFoundException)
                {
                    await next();
                    GenerateCacheHeadersAsync(ctx);
                }
            }
        }

        private async void GenerateCacheHeadersAsync(HttpContext context)
        {
            context.Response.GetTypedHeaders().CacheControl = new CacheControlHeaderValue
            {
                Public = true,
                MaxAge = TimeSpan.FromSeconds(MaxAge)
            };
            // context.Response.GetTypedHeaders().ContentType = new MediaTypeHeaderValue("text/html");
            // context.Response.GetTypedHeaders().ContentLength = Encoding.ASCII.GetBytes(content).LongLength;
            context.Response.GetTypedHeaders().ETag = await _eTagGenerator.GenerateETag(context);
            var responseCachingFeature = context.Features.Get<IResponseCachingFeature>();
            responseCachingFeature.VaryByQueryKeys = VaryByQueryKeys;
        }

        private bool CheckSupportedMethods(HttpContext context)
        {
            if (!_supportedMethods.Select(method => method.ToString()).Contains(context.Request.Method))
            {
                throw new MethodUnsupportedException($"{context.Request.Method} is not supported for caching purposes");
            }

            return true;
        }

        private async Task<Tuple<string, string>> GetImageSpecsAsync(HttpContext context)
        {
            var imageName = context.Request.Path.Value.Split('/').Last();
            var imagePath = await _imageSource.GetImageFullPath(imageName);
            return new Tuple<string, string>(imageName, imagePath);
        }
    }

    class GeneralCacheAttribute : TypeFilterAttribute
    {
        public GeneralCacheAttribute() : base(typeof(GeneralCacheFilter))
        {
        }
    }
}