using Applitools.Selenium.Capture;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestDom : ReportingTestSuite
    {
        [Test]
        public void TestDomSerialization()
        {
            string expectedDomJson = CommonUtils.ReadResourceFile("Test.Eyes.Selenium.DotNet.Resources.ExpectedDom_CorsCssTestPage.json");
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
            driver.Url = "https://applitools.github.io/demo/TestPages/CorsCssTestPage/";
            try
            {
                UserAgent ua = eyes.seleniumEyes_.userAgent_;
                DomCapture domCapture = new DomCapture(eyes.Logger, eyesWebDriver, ua);
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
