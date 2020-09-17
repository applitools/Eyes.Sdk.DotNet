namespace Applitools
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that a new test (i.e., a test for which no baseline exists) ended.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design", 
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "Not relevant for this exception")]
    [ComVisible(true)]
    [Serializable]
    public class NewTestException : TestFailedException
    {
        /// <summary>
        /// Creates an <see cref="NewTestException"/> instance.
        /// </summary>
        public NewTestException()
        {
        }

        /// <summary>
        /// Creates an <see cref="NewTestException"/> instance.
        /// </summary>
        public NewTestException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Creates an <see cref="NewTestException"/> instance.
        /// </summary>
        public NewTestException(TestResults testResults, string message)
            : base(testResults, message)
        {
        }

        /// <summary>
        /// Creates an <see cref="NewTestException"/> instance.
        /// </summary>
        protected NewTestException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
