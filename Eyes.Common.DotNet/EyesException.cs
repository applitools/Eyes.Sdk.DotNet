namespace Applitools
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Applitools Eyes Exception.
    /// </summary>
    [ComVisible(true)]
    [Serializable]
    public class EyesException : Exception
    {
        /// <summary>
        /// Creates an <see cref="EyesException"/> instance.
        /// </summary>
        public EyesException()
        {
        }

        /// <summary>
        /// Creates an <see cref="EyesException"/> instance.
        /// </summary>
        public EyesException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an <see cref="EyesException"/> instance.
        /// </summary>
        public EyesException(string message, Exception ex)
            : base(message, ex)
        {
        }

        /// <summary>
        /// Creates an <see cref="EyesException"/> instance.
        /// </summary>
        protected EyesException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
