namespace Applitools.Utils
{
    using System;
    using System.Runtime.Serialization;

    /// <summary>
    /// The base class of all Applitools' custom exceptions. 
    /// Indicates a general unexpected failure to complete an operation.
    /// </summary>
    [Serializable]
    public class OperationFailedException : Exception
    {
        /// <inheritdoc/>
        public OperationFailedException() : base() 
        { 
        }

        /// <inheritdoc/>
        public OperationFailedException(string message) : base(message) 
        {
        }

        /// <inheritdoc/>
        public OperationFailedException(string message, Exception innerException)
            : base(message, innerException) 
        {
        }

        /// <inheritdoc/>
        protected OperationFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context) 
        {
        }
    }
}
