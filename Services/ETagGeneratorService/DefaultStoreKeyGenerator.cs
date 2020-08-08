using System.Linq;
using System.Threading.Tasks;
using FakePhoto.Services.ETagGeneratorService.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FakePhoto.Services.ETagGeneratorService
{
    public class DefaultStoreKeyGenerator : IStoreKeyGenerator
    {
        public Task<StoreKey> GenerateStoreKey(HttpContext context)
        {
            // generate a key to store the entity tag with in the entity tag store
            var requestHeaderValues = context.Request
                .Headers
                .ToList()
                .SelectMany(h => h.Value);

            var resourcePath = context.Request.Path.ToString();
            var queryString = context.Request.QueryString.ToString();
            var responseBody = context.Response.Body.ToString();

            return Task.FromResult(new StoreKey
            {
                {nameof(resourcePath), resourcePath},
                {nameof(queryString), queryString},
                {nameof(responseBody), responseBody},
                {nameof(requestHeaderValues), string.Join("-", requestHeaderValues)}
            });
        }
    }
}