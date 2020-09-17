using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class TestResultEqualityTest
    {
        [Test]
        public void Test_TestResult_Equal()
        {
            TestResult r1 = new TestResult("some test name", false, new Dictionary<string, object>() { { "arg1", 1.5 }, { "arg2", "some string" } });
            TestResult r2 = new TestResult("some test name", true, new Dictionary<string, object>() { { "arg1", 1.5 }, { "arg2", "some string" } });
            Assert.AreEqual(r1, r2);
        }

        [Test]
        public void Test_TestResult_NotEqual()
        {
            TestResult r1 = new TestResult("some test name", false, new Dictionary<string, object>() { { "arg1", 1.5 }, { "arg2", "some string" } });
            TestResult r2 = new TestResult("some test name", true, new Dictionary<string, object>() { { "arg1", 1.5 }, { "arg2", "some string" }, { "arg3", null } });
            Assert.AreNotEqual(r1, r2);
        }
    }
}
