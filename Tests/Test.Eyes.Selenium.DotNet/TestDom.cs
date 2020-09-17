using Applitools.Selenium.Capture;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestDom
    {
        //[Test]
        public void TestDomSerialization()
        {
            string expectedDomJson = CommonUtils.ReadResourceFile("Applitools.Selenium.Tests.Resources.ExpectedDom_CorsTestPage.json");
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            Eyes eyes = new Eyes();
            Configuration conf = new Configuration();
            conf.SetAppName("app");
            conf.SetTestName("test");
            conf.SetViewportSize(800, 600);
            conf.SetHideScrollbars(true);
            conf.SetBatch(TestDataProvider.BatchInfo);
            eyes.SetConfiguration(conf);
            eyes.SetLogHandler(TestUtils.InitLogHandler(nameof(TestDom)));
            EyesWebDriver eyesWebDriver = (EyesWebDriver)eyes.Open(driver);
            driver.Url = "https://applitools.github.io/demo/TestPages/CorsTestPage/";
            Thread.Sleep(4000);
            try
            {
                DomCapture domCapture = new DomCapture(eyes.Logger, eyesWebDriver);
                string json = domCapture.GetFullWindowDom();

                object expectedJsonObj = JsonConvert.DeserializeObject(expectedDomJson);
                object jsonObj = JsonConvert.DeserializeObject(json);

                expectedDomJson = JsonConvert.SerializeObject(expectedJsonObj, Formatting.Indented);
                json = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);

                Assert.AreEqual(expectedDomJson, json);
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }
    }
}
