using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using FakePhoto.Services.ETagGeneratorService.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;

namespace FakePhoto.Services.ETagGeneratorService
{
    public class DefaultETagGenerator : IETagGenerator
    {
        private readonly IStoreKeyGenerator _storeKeyGenerator;
        private readonly ETagType _eTagType;

        public DefaultETagGenerator(IStoreKeyGenerator storeKeyGenerator, ETagType eTagType = ETagType.Strong)
        {
            _storeKeyGenerator = storeKeyGenerator;
            _eTagType = eTagType;
        }

        // Key = generated from request URI & headers (if VaryBy is set, only use those headers)
        public async Task<EntityTagHeaderValue> GenerateETag(HttpContext context)
        {
            var storeKey = (await _storeKeyGenerator.GenerateStoreKey(context)).ToString();
            var requestKeyAsBytes = Encoding.UTF8.GetBytes(storeKey);

            // combine both to generate an etag
            var combinedBytes = GetETagValue(requestKeyAsBytes);
            return new ETag(_eTagType, combinedBytes).GetETag();
        }

        private static string GetETagValue(byte[] a)
        {
            using var md5 = MD5.Create();
            var hash = md5.ComputeHash(a);
            var hex = BitConverter.ToString(hash);
            return hex.Replace("-", "");
        }
    }
}