using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;

namespace FakePhoto
{
    public interface IFakePhotoService
    {
        // HttpClient Client { get; }
        Task<byte[]> GetBytePhotoByDimensions(Tuple<int, int> dimensions);
    }

    public class FakePhotoService : IFakePhotoService
    {
        private HttpClient Client { get; }

        public FakePhotoService(HttpClient client)
        {
            Client = client;
            InitClient();
        }

        private void InitClient()
        {
            Client.BaseAddress = new Uri("https://fakeimg.pl/");
            // Client.DefaultRequestHeaders.Clear();
            Client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("Mozilla", "5.0")));
            Client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("AppleWebKit", "537.36")));
            Client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("Chrome", "84.0.4147.105")));
            Client.DefaultRequestHeaders.UserAgent.Add(
                new ProductInfoHeaderValue(new ProductHeaderValue("Safari", "537.36")));
        }

        public async Task<byte[]> GetBytePhotoByDimensions(Tuple<int, int> dimensions)
        {
            var response = await Client.GetAsync($"{dimensions.Item1}x{dimensions.Item2}/");
            var statusCode = (int) response.StatusCode;
            if (statusCode >= 400)
            {
                throw new HttpRequestException($"Error {statusCode} when request the page");
            }

            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}