using System.Collections.Generic;

namespace FakePhoto.Services.ETagGeneratorService
{
    public class StoreKey : Dictionary<string, string>
    {
        public override string ToString() => string.Join("-", Values);
    }
}