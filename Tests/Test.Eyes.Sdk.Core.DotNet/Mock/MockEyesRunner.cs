using System.Collections.Generic;

namespace Applitools.Tests
{
    internal class MockEyesRunner : EyesRunner
    {
        protected override IEnumerable<EyesBase> GetAllEyes()
        {
            return null;
        }

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            return null;
        }
    }
}