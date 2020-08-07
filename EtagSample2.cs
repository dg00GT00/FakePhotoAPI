using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;

namespace FakePhoto
{
    /// <summary>
    /// Enables HTTP Response CacheControl management with ETag values.
    /// </summary>
    public class ClientCacheWithEtagAttribute : ActionFilterAttribute
    {
        private readonly TimeSpan _clientCache;

        private readonly HttpMethod[] _supportedRequestMethods =
        {
            HttpMethod.Get,
            HttpMethod.Head
        };

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="clientCacheInSeconds">Indicates for how long the client should cache the response. The value is in seconds</param>
        public ClientCacheWithEtagAttribute(int clientCacheInSeconds)
        {
            _clientCache = TimeSpan.FromSeconds(clientCacheInSeconds);
        }

        public async Task OnActionExecutedAsync(ActionExecutedContext actionExecutedContext,
            CancellationToken cancellationToken)
        {
            if (!_supportedRequestMethods.Contains(actionExecutedContext.HttpContext.Request.Method))
            {
                return;
            }

            if (actionExecutedContext.HttpContext.Response?.Content == null)
            {
                return;
            }

            var body = await actionExecutedContext.HttpContext.Response.Content.ReadAsStringAsync();
            if (body == null)
            {
                return;
            }

            var computedEntityTag = GetETag(Encoding.UTF8.GetBytes(body));

            if (actionExecutedContext.HttpContext.Request.Headers.IfNoneMatch.Any()
                && actionExecutedContext.HttpContext.Request.Headers.IfNoneMatch.First().Tag.Trim('"')
                    .Equals(computedEntityTag, StringComparison.InvariantCultureIgnoreCase))
            {
                actionExecutedContext.HttpContext.Response.StatusCode = HttpStatusCode.NotModified;
                actionExecutedContext.HttpContext.Response.Content = null;
            }

            var cacheControlHeader = new CacheControlHeaderValue
            {
                Private = true,
                MaxAge = _clientCache
            };

            actionExecutedContext.HttpContext.Response.Headers.ETag = new EntityTagHeaderValue($"\"{computedEntityTag}\"", false);
            actionExecutedContext.HttpContext.Response.Headers.CacheControl = cacheControlHeader;
        }

        private static string GetETag(byte[] contentBytes)
        {
            using (var md5 = MD5.Create())
            {
                var hash = md5.ComputeHash(contentBytes);
                string hex = BitConverter.ToString(hash);
                return hex.Replace("-", "");
            }
        }
    }
}