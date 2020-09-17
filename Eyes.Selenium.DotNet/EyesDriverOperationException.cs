namespace Applitools.Selenium
{
    using System;
    using System.Runtime.Serialization;

    [Serializable]
    public class EyesDriverOperationException : EyesException
    {
        public EyesDriverOperationException()
        {
        }

        public EyesDriverOperationException(string message) : base(message)
        {
        }

        public EyesDriverOperationException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected EyesDriverOperationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}