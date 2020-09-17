namespace Applitools.Exceptions
{
    public class DiffsFoundException : TestFailedException
    {
        /// <summary>
        /// Creates a new <see cref="DiffsFoundException"/> instance.
        /// </summary>
        public DiffsFoundException(TestResults testResults, string test, string app, string url)
            : base(testResults, string.Format("Test '{0}' of '{1}' detected differences! See details at: {2}", test, app, url))
        {
        }
    }
}
