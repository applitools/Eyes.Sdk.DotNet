namespace Applitools
{
    using Applitools.Utils.Geometry;
    using NUnit.Framework;
    using System.Text;

    [TestFixture]
    public class TestAppOutput
    {
        [Test]
        public void TestCtor()
        {
            byte[] contentBytes = Encoding.UTF8.GetBytes("some fake base64 screenshot serialized");
            AppOutput ao = new AppOutput("title", new Location(10, 30), contentBytes, "abc");
            Assert.AreEqual("title", ao.Title);
            Assert.AreEqual(new Location(10, 30), ao.Location);
            Assert.AreEqual(contentBytes, ao.ScreenshotBytes);
            Assert.AreEqual("abc", ao.ScreenshotUrl);
        }
    }
}
