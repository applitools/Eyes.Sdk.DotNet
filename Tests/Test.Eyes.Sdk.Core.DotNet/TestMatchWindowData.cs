using Applitools.Tests.Utils;
using Applitools.Utils.Geometry;
using NUnit.Framework;
using System;
using System.Text;

namespace Applitools
{
    [TestFixture]
    public class TestMatchWindowData : ReportingTestSuite
    {
        private Trigger trigger_;
        private AppOutput appOut_;

        [SetUp]
        public void Setup()
        {
            trigger_ = new MouseTrigger(MouseAction.Click, new Region(0, 0, 0, 0), new Location(0, 0));
            appOut_ = new AppOutput("Dummy", new Location(20, 40), Encoding.UTF8.GetBytes("blob as base64"), "bla");
        }

        [Test]
        public void TestCtor()
        {
            MatchWindowData mwd = new MatchWindowData(appOut_, "mytag");
            ImageMatchSettings ims = new ImageMatchSettings();
            mwd.Options = new ImageMatchOptions(ims);
            mwd.Options.UserInputs = new Trigger[] { trigger_ };
            Assert.AreEqual(appOut_, mwd.AppOutput);
            Assert.AreEqual(new Trigger[] { trigger_ }, mwd.Options.UserInputs);
            Assert.AreEqual("mytag", mwd.Tag);
            new MatchWindowData(appOut_, "tag");
            Assert.Throws<ArgumentNullException>(() => new MatchWindowData(null, "tag"));
            var ok = new MatchWindowData(appOut_, null);
        }
    }
}
