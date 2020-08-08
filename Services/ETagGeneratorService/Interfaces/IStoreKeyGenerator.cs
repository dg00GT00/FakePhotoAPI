using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace FakePhoto.Services.ETagGeneratorService.Interfaces
{
    public interface IStoreKeyGenerator
    {
        Task<StoreKey> GenerateStoreKey(HttpContext context);
    }
}