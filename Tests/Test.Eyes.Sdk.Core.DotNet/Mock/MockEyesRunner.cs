using Applitools.Tests.Utils;
using System.Collections.Generic;

namespace Applitools.Tests
{
    internal class MockEyesRunner : EyesRunner
    {
        public MockEyesRunner() : base(new NunitLogHandler())
        {

        }

        protected override IEnumerable<IEyesBase> GetAllEyes()
        {
            return null;
        }

        protected override TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            return null;
        }
    }
}