using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;

namespace Applitools.Selenium.Tests
{
    public class TestMobileEmulation : ReportingTestSuite
    {
        [Test]
        public void TestCheckRegion_LoadPageAfterOpen()
        {
            Eyes eyes = null;
            IWebDriver webDriver = null;
            try
            {
                ChromeMobileEmulationDeviceSettings mobileSettings = new ChromeMobileEmulationDeviceSettings()
                {
                    UserAgent = "Mozilla/5.0 (Linux; Android 8.0.0; Android SDK built for x86_64 Build/OSR1.180418.004) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/69.0.3497.100 Mobile Safari/537.36",
                    Width = 384,
                    Height = 512,
                    PixelRatio = 2
                };

                ChromeOptions chromeOptions = new ChromeOptions();
                chromeOptions.EnableMobileEmulation(mobileSettings);
                webDriver = SeleniumUtils.CreateChromeDriver(chromeOptions);


                eyes = new Eyes();
                Configuration configuration = eyes.GetConfiguration();
                configuration.SetAppName(nameof(TestMobileEmulation)).SetTestName(nameof(TestCheckRegion_LoadPageAfterOpen));
                eyes.SetConfiguration(configuration);
                eyes.Open(webDriver);

                webDriver.Url = "https://applitools.github.io/demo/TestPages/SpecialCases/hero.html";
                
                eyes.Check(Target.Region(By.CssSelector("img")).Fully().WithName("Element outside the viewport"));
                eyes.Close();
            }
            finally
            {
                eyes?.AbortIfNotClosed();
                webDriver?.Quit();
            }
        }
    }
}
