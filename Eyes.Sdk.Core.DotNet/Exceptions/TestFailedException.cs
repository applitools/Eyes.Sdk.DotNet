namespace Applitools
{
    using System;
    using System.Runtime.InteropServices;
    using System.Runtime.Serialization;

    /// <summary>
    /// Indicates that a test did not pass (i.e., test either 
    /// failed or is a new test).
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Usage", 
        "CA2240:ImplementISerializableCorrectly",
        Justification = "Will not be serialized")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage(
        "Microsoft.Design", 
        "CA1032:ImplementStandardExceptionConstructors",
        Justification = "Not needed for this exception")]
    [ComVisible(true)]
    [Serializable]
    public class TestFailedException : Exception
    {
        /// <summary>
        /// Creates a new <see cref="TestFailedException"/> instance.
        /// </summary>
        public TestFailedException()
        {
        }

        /// <summary>
        /// Creates a new <see cref="TestFailedException"/> instance.
        /// </summary>
        public TestFailedException(TestResults testResults, string message)
            : base(message)
        {
            TestResults = testResults;
        }

        /// <summary>
        /// Creates a new <see cref="TestFailedException"/> instance.
        /// </summary>
        public TestFailedException(string message)
            : this(null, message)
        {
        }

        /// <summary>
        /// Creates a new <see cref="TestFailedException"/> instance.
        /// </summary>
        protected TestFailedException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the <see cref="T:Applitools.Eyes.TestResults"/> 
        /// of the failed test or <c>null</c> if the test has not yet 
        /// ended (e.g., when thrown by <see cref="M:Applitools.Eyes.CheckWindow()"/>).
        /// </summary>
        public TestResults TestResults { get; private set; }
    }
}
