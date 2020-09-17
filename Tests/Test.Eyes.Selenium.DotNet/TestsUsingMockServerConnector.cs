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
            IServerConnectorFactory serverConnectorFactory = new MockServerConnectorFactory();
            Eyes eyes = new Eyes(serverConnectorFactory);
            eyes.Batch = TestDataProvider.BatchInfo;
            TestUtils.SetupLogging(eyes);
            MockServerConnector mockServerConnector = (MockServerConnector)eyes.seleniumEyes_.ServerConnector;
            mockServerConnector.AsExcepted = false;
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
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
            byte[] screenshotBytes = mockServerConnector.MatchWindowCalls[0].AppOutput.ScreenshotBytes;
            string finalImageHash = screenshotBytes.GetSha256Hash();
            if (finalImageHash != "1C2E890355934CF44F070D1F8199E0AF390275E9D82ED82A898C8172406555E9" && // Windows
                finalImageHash != "0DC273FEF58C6E4B2563DF2078347B32EFC702E96E11D782E3664F0690881E6B" && // Windows with different Antialiasing
                finalImageHash != "1FCC19630668582400BD59119FDF4692707B67A524DEB2F8047D96C9E56CC074" && // Linux, Chrome <85
                finalImageHash != "FCBE40C10CFD01E9F86084F655708226E53B98508F1696CBAEEF158CA20CDD78") // Linux, Chrome >=85
            {
                Assert.Fail("Different image than expected. Hash: {0}\nBase64:\n{1}\n", finalImageHash,
                    string.Join('\n', Convert.ToBase64String(screenshotBytes).Split(160)));
            }
        }
    }
}
