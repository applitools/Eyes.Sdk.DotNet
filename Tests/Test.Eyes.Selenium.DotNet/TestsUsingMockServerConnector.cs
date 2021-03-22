using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class TestsUsingMockServerConnector : ReportingTestSuite
    {
        [Test]
        public void TestFullWindowInCorrectLocationAfterRetry()
        {
            WebDriverProvider webdriverProvider = new WebDriverProvider();
            IServerConnectorFactory serverConnectorFactory = new MockServerConnectorFactory(webdriverProvider);
            Eyes eyes = new Eyes(serverConnectorFactory);
            eyes.Batch = TestDataProvider.BatchInfo;
            TestUtils.SetupLogging(eyes);
            MockServerConnector mockServerConnector = (MockServerConnector)eyes.seleniumEyes_.ServerConnector;
            mockServerConnector.AsExpected = false;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            webdriverProvider.SetDriver(driver);
            driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
            try
            {
                eyes.Open(driver, "DotNet Tests", nameof(TestFullWindowInCorrectLocationAfterRetry), new Size(700, 460));
                driver.FindElement(By.TagName("input")).SendKeys("Different Input");
                eyes.Check(Target.Window().Fully());
                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                driver.Quit();
            }
            Assert.AreEqual(1, mockServerConnector.MatchWindowCalls.Count, "Matches");
            Assert.AreEqual(0, mockServerConnector.ReplaceMatchedStepCalls.Count, "Replacements");
            byte[] screenshotBytes = mockServerConnector.ImagesAsByteArrays[0];
            string finalImageHash = screenshotBytes.GetSha256Hash();
            if (finalImageHash != "0EEBFD0FCDE40BA1FB5D4B8C4DA535F3C846B2E1685C84488E43FC6DC6ECD88A" && // Windows
                finalImageHash != "5E771AB31BA0D3232E370FDA5E630A649E0E03FA704243BBD288303B4D9064B3" && // Windows with different Antialiasing
                finalImageHash != "35D38A5CE5571F4D2AB4C543FB7CDB2EDC85A82C2472A8F062700637DE8BCF30" && // Windows, Chrome >= 87
                finalImageHash != "CAE9ADB9A45932BF9DEA51972A6B69908D012324E0CB4B2A93368CDD3C8E56D2" && // Linux, Chrome >=85
                finalImageHash != "993DB6CB4EC9D93AB5A7F0F598F179DADBB562A5E6A31BFAA6211C809BA5C9BB" && // Linux, Chrome >=87
                finalImageHash != "53CD94D4450FF82825C8B36A04E0BC68B32BACFB4F09019294D907AF0080F6E5")   // Linux, Chrome >=89
            {
                Assert.Fail("Different image than expected. Hash: {0}\nBase64:\n{1}\n", finalImageHash,
                    string.Join("\n", Convert.ToBase64String(screenshotBytes).Split(160)));
            }
        }
    }
}
