using Applitools.Tests.Utils;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace Applitools.Generated.Selenium.Tests
{
    public abstract class ReportingTestSuiteGenerratedAppium : ReportingTestSuite
    {
        protected override TestResult GetTestResult()
        {
            TestContext tc = TestContext.CurrentContext;
            TestStatus status = tc.Result.Outcome.Status;
            bool passed = status == TestStatus.Passed;
            string name= tc.Test.MethodName;
            return new TestResultGeneratedAppium(name, passed, GetTestParameters());
        }
    }
}
