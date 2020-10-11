using Applitools.Tests.Utils;
using Applitools.Utils;
using Applitools.VisualGrid;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    public class TestDeviceInfo : ReportingTestSuite
    {
        [Test]
        public void TestIosDeviceInfo()
        {
            JsonSerializer serializer = JsonUtils.CreateSerializer();

            IosDeviceInfo iosDeviceInfo = new IosDeviceInfo(IosDeviceName.iPhone_7);
            JObject obj = (JObject)JsonConvert.DeserializeObject(serializer.Serialize(iosDeviceInfo));
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual("iPhone 7", obj.Value<string>("name"));
            Assert.AreEqual("portrait", obj.Value<string>("screenOrientation"));


            iosDeviceInfo = new IosDeviceInfo(IosDeviceName.iPhone_11, ScreenOrientation.Landscape);
            obj = (JObject)JsonConvert.DeserializeObject(serializer.Serialize(iosDeviceInfo));
            Assert.AreEqual(2, obj.Count);
            Assert.AreEqual("iPhone 11", obj.Value<string>("name"));
            Assert.AreEqual("landscape", obj.Value<string>("screenOrientation"));

            iosDeviceInfo = new IosDeviceInfo(IosDeviceName.iPhone_8, iosVersion: IosVersion.ONE_VERSION_BACK);
            obj = (JObject)JsonConvert.DeserializeObject(serializer.Serialize(iosDeviceInfo));
            Assert.AreEqual(3, obj.Count);
            Assert.AreEqual("iPhone 8", obj.Value<string>("name"));
            Assert.AreEqual("portrait", obj.Value<string>("screenOrientation"));
            Assert.AreEqual("latest-1", obj.Value<string>("version"));

            iosDeviceInfo = new IosDeviceInfo(IosDeviceName.iPhone_X, ScreenOrientation.Landscape, IosVersion.LATEST);
            obj = (JObject)JsonConvert.DeserializeObject(serializer.Serialize(iosDeviceInfo));
            Assert.AreEqual(3, obj.Count);
            Assert.AreEqual("iPhone X", obj.Value<string>("name"));
            Assert.AreEqual("landscape", obj.Value<string>("screenOrientation"));
            Assert.AreEqual("latest", obj.Value<string>("version"));
        }
    }
}
