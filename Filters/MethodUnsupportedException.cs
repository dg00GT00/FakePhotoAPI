using System;

namespace FakePhoto.Filters
{
    [Serializable]
    public class MethodUnsupportedException : Exception
    {
        public MethodUnsupportedException()
        {
        }

        public MethodUnsupportedException(string message) : base(message)
        {
        }

        public MethodUnsupportedException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}