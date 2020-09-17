namespace Applitools
{
    using Applitools;
    using NUnit.Framework;

    [TestFixture]
    public class TestMatchResult
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
