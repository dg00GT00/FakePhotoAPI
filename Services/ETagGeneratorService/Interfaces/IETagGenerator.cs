using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FakePhoto.Services.ETagGeneratorService.Interfaces
{
    public interface IETagGenerator
    {
        /// <summary>
        /// Contract for an E-Tag Generator, used to generate the unique weak or strong E-Tags for cache items
        /// </summary>
        Task<EntityTagHeaderValue> GenerateETag(HttpContext context);
    }
}