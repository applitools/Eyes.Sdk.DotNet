namespace Applitools.Utils
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that a component does not exist, not accessible or not functional
    /// </summary>
    [Serializable]
    public class NotAvailableException : OperationFailedException
    {
        /// <inheritdoc/>
        public NotAvailableException() : base()
        {
        }

        /// <inheritdoc/>
        public NotAvailableException(string message) : base(message) 
        {
        }

        /// <inheritdoc/>
        public NotAvailableException(string message, Exception innerException) 
            : base(message, innerException) 
        {
        }

        /// <inheritdoc/>
        protected NotAvailableException(SerializationInfo info, StreamingContext context) 
            : base(info, context) 
        {
        }
    }
}
