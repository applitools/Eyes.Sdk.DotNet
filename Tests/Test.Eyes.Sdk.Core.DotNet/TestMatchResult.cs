using Applitools.Tests.Utils;
using NUnit.Framework;

namespace Applitools
{
    [TestFixture]
    public class TestMatchResult : ReportingTestSuite
    {
        [Test]
        public void Test1()
        {
            MatchResult mr = new MatchResult();
            Assert.AreEqual(false, mr.AsExpected);
            Assert.IsNull(mr.WindowId);
            mr.WindowId = "dummy win id";
            Assert.AreEqual("dummy win id", mr.WindowId);
            mr.AsExpected = true;
            Assert.AreEqual(true, mr.AsExpected);
        }
    }
}
