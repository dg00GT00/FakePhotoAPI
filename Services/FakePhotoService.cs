using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FakePhoto.Services
{
    public interface IFakePhotoService
    {
        Task<byte[]> GetBytePhotoByDimensions(Tuple<int, int> dimensions);

        public static void InitClient(HttpClient client)
        {
            client.DefaultRequestHeaders.Clear();
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("Mozilla", "5.0")));
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("AppleWebKit", "537.36")));
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("Chrome", "84.0.4147.105")));
            client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("Safari", "537.36")));
        }
    }

    public class FakePhotoService : IFakePhotoService
    {
        private readonly HttpClient _client;

        public FakePhotoService(HttpClient client)
        {
            _client = client;
            IFakePhotoService.InitClient(client);
        }

        public async Task<byte[]> GetBytePhotoByDimensions(Tuple<int, int> dimensions)
        {
            var response = await _client.GetAsync($"{dimensions.Item1}x{dimensions.Item2}/");
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}