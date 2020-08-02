using FakePhoto.Middlewares;
using Microsoft.AspNetCore.Builder;

namespace FakePhoto.Extensions
{
    public static class CacheImageExtensions
    {
        public static IApplicationBuilder UseImageCache(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CacheImageMiddleware>();
        }
    }
}