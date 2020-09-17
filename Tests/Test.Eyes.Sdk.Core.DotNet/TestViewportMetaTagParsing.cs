using Applitools.Utils;
using NUnit.Framework;

namespace Applitools.Tests
{
    public class TestViewportMetaTagParsing
    {
        [TestCase("width=device-width, initial-scale=1, user-scalable=no, minimum-scale=1.0", true, 0f, 1.0f)]
        [TestCase("initial-scale=1,width=device-width,      user-scalable=no", true, 0f, 1.0f)]
        [TestCase("initial-scale=1.3,width=500,    user-scalable=yes", false, 500f, 1.3f)]
        [TestCase("width=960px", false, 960f, 0.0f)]
        [TestCase("width=device-width", true, 0, 0.0f)]
        [TestCase("", false, 0f, 0.0f)]
        public void TestParseViewportMetaTag(string viewportMetaTag, bool expectedFollowDeviceWidth, float expectedDeviceWidth, float expectedInitialScale)
        {
            ViewportMetaTag data = ViewportMetaTag.ParseViewportMetaTag(viewportMetaTag);
            Assert.AreEqual(expectedFollowDeviceWidth, data.FollowDeviceWidth, "FollowDeviceWidth");
            Assert.AreEqual(expectedDeviceWidth, data.DeviceWidth, "DeviceWidth");
            Assert.AreEqual(expectedInitialScale, data.InitialScale, "InitialScale");
        }
    }
}
