namespace Applitools.Tests
{
    internal class MockEyesRunner : EyesRunner
    {
        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            return null;
        }
    }
}