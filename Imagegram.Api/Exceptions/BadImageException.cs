using System;
using System.Runtime.Serialization;

namespace Imagegram.Api.Exceptions
{
    [Serializable]
    internal class BadImageException : Exception
    {
        public BadImageException()
        {
        }

        public BadImageException(string message) : base(message)
        {
        }

        public BadImageException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected BadImageException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}