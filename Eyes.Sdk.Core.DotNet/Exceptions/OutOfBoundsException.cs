using System;
using System.Runtime.Serialization;

namespace Applitools
{
    [Serializable]
    public class OutOfBoundsException : Exception
    {
        public OutOfBoundsException()
        {
        }

        public OutOfBoundsException(string message) : base(message)
        {
        }

        public OutOfBoundsException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected OutOfBoundsException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}