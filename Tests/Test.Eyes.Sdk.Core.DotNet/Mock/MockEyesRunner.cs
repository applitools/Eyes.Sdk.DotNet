using Applitools.Tests.Utils;
using System.Collections.Generic;
using Applitools.Selenium.Commands.Requests;

namespace Applitools.Tests
{
    internal class MockEyesRunner : EyesRunner
    {
        public MockEyesRunner() : base(new NunitLogHandler())
        {

        }

        protected override ManagerConfig InitConfig()
        {
            throw new System.NotImplementedException();
        }

        protected override IEnumerable<IEyesBase> GetAllEyes()
        {
            return null;
        }

        protected  TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException)
        {
            return null;
        }
    }
}