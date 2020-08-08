using Microsoft.Net.Http.Headers;

namespace FakePhoto.Services.ETagGeneratorService
{
    public class ETag
    {
        public ETagType ETagType { get; }
        public string Value { get; }

        public ETag(ETagType eTagType, string value)
        {
            ETagType = eTagType;
            Value = value;
        }

        public EntityTagHeaderValue GetETag()
        {
            return ETagType switch
            {
                ETagType.Strong => new EntityTagHeaderValue(Value, false),
                ETagType.Weak => new EntityTagHeaderValue(Value, true),
                _ => new EntityTagHeaderValue(Value, false)
            };
        }
    }
}