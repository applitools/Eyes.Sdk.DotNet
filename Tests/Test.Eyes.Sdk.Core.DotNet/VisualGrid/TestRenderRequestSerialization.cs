using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.Utils;
using Applitools.VisualGrid.Model;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Applitools.VisualGrid
{
    [TestFixture]
    public class TestRenderRequestSerialization : ReportingTestSuite
    {
        [Test]
        public void TestSerialization()
        {
            Uri webHook = new Uri("https://some.uri.com");
            Uri url = new Uri("https://another.url.co.il");
            Uri stitchingServiceUrl = new Uri("https://some.stitchingserviceuri.com");
            RGridDom dom = new RGridDom();
            Dictionary<string, RGridResource> resources = new Dictionary<string, RGridResource>();
            int width = 1600;
            int height = 1200;
            SizeMode target = SizeMode.FullPage;
            VisualGridSelector selector = null;
            System.Drawing.Rectangle region = new System.Drawing.Rectangle(40, 50, 60, 70);
            EmulationBaseInfo emulationInfo = new ChromeEmulationInfo(DeviceName.Galaxy_S5, ScreenOrientation.Portrait);
            RenderInfo renderInfo = new RenderInfo(width, height, target, selector, region, emulationInfo, null);
            string platform = "android";
            BrowserType browserName = BrowserType.IE_10;
            object scriptHooks = null;
            string xpath = "//html/body/some/path/to/some/element[@with:attribute]";
            object category = "cat";
            VisualGridSelector[] selectorsToFindRegionsFor = new VisualGridSelector[]{
                new VisualGridSelector(xpath, category)
            };
            bool sendDom = true;

            RenderRequest request = new RenderRequest("id", webHook, url, stitchingServiceUrl, dom, resources,
                renderInfo, platform, browserName, scriptHooks, selectorsToFindRegionsFor, sendDom, null, null, null);

            JsonSerializerSettings settings = JsonUtils.CreateSerializerSettings();
            settings.Formatting = Formatting.Indented;
            string json = JsonConvert.SerializeObject(request, settings);
            string expectedJson = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.TestRenderRequestSerialization.json");
            Assert.AreEqual(expectedJson, json);
        }

        [Test]
        public void TestSerializationIosSimulator()
        {
            Uri url = new Uri("https://another.url.co.il");
            Uri stitchingServiceUrl = new Uri("https://another.stitchingserviceuri.co.il");
            RGridDom dom = new RGridDom();
            Dictionary<string, RGridResource> resources = new Dictionary<string, RGridResource>();
            SizeMode target = SizeMode.FullPage;
            VisualGridSelector selector = null;
            System.Drawing.Rectangle region = new System.Drawing.Rectangle(40, 50, 60, 70);
            IosDeviceInfo deviceInfo = new IosDeviceInfo(IosDeviceName.iPhone_XR, ScreenOrientation.Landscape, IosVersion.ONE_VERSION_BACK);
            RenderInfo renderInfo = new RenderInfo(0, 0, target, selector, region, null, deviceInfo);
            VisualGridSelector[] selectorsToFindRegionsFor = new VisualGridSelector[0];
            bool sendDom = true;
            RenderBrowserInfo browserInfo = new RenderBrowserInfo(deviceInfo);

            RenderRequest request = new RenderRequest("id", null, url, stitchingServiceUrl, dom, resources, renderInfo,
                browserInfo.Platform, browserInfo.BrowserType, null, selectorsToFindRegionsFor, sendDom, null, null, null);

            JsonSerializerSettings settings = JsonUtils.CreateSerializerSettings();
            settings.Formatting = Formatting.Indented;
            string json = JsonConvert.SerializeObject(request, settings);
            string expectedJson = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.TestRenderRequestSerializationIosDevice.json");
            Assert.AreEqual(expectedJson, json);
        }

        //[Test]
        public void TestEmulatedDeviceSizesDeserialization()
        {
            string sourceJson = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.EmulatedDevicesSizes.json");
            var obj = JsonConvert.DeserializeObject<Dictionary<string, DeviceSize>>(sourceJson);
            Dictionary<DeviceName, DeviceSize> result = CommonUtils.ConvertDictionaryKeys<DeviceName, DeviceSize>(obj);
            Assert.AreEqual(Enum.GetValues(typeof(DeviceName)).Length, result.Keys.Count);
        }

        [Test]
        public void TestIosDeviceSizesDeserialization()
        {
            string sourceJson = CommonUtils.ReadResourceFile("Test.Eyes.Sdk.Core.DotNet.Resources.IosDevicesSizes.json");
            var obj = JsonConvert.DeserializeObject<Dictionary<string, DeviceSize>>(sourceJson);
            Dictionary<IosDeviceName, DeviceSize> result = CommonUtils.ConvertDictionaryKeys<IosDeviceName, DeviceSize>(obj);
            Assert.AreEqual(Enum.GetValues(typeof(IosDeviceName)).Length, result.Keys.Count);
        }
    }
}
