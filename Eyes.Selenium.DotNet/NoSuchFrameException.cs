using System;
using System.Runtime.Serialization;

namespace Applitools.Selenium
{
    [Serializable]
    internal class NoSuchFrameException : Exception
    {
        public NoSuchFrameException()
        {
        }

        public NoSuchFrameException(string message) : base(message)
        {
        }

        public NoSuchFrameException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected NoSuchFrameException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}