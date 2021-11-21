using System;
using System.Runtime.Serialization;

namespace Applitools.OpenQA.Selenium
{
    [Serializable]
    internal class InvalidSelectorException : Exception
    {
        public InvalidSelectorException()
        {
        }

        public InvalidSelectorException(string message) : base(message)
        {
        }

        public InvalidSelectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected InvalidSelectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}