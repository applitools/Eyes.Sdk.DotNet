using Applitools.Selenium.Tests.Mock;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class TestExceptions : ReportingTestSuite
    {
        [TestCase(true)]
        [TestCase(false)]
        public void TestEyesExceptions(bool useVG)
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            WebDriverProvider driverProvider = new WebDriverProvider();
            driverProvider.SetDriver(driver);
            MockServerConnectorFactory serverConnectorFactory = new MockServerConnectorFactory(driverProvider);
            ILogHandler logHandler = TestUtils.InitLogHandler();
            EyesRunner runner = useVG
                ? (EyesRunner)new VisualGridRunner(10, null, serverConnectorFactory, logHandler)
                : new ClassicRunner(logHandler, serverConnectorFactory);
            Eyes eyes = new Eyes(runner);
            try
            {
                eyes.ApiKey = "";
                Assert.That(() => { eyes.Open(driver); }, Throws.Exception.With.InstanceOf<EyesException>().With.Property("Message").EqualTo("API key not set! Log in to https://applitools.com to obtain your API key and use the 'Eyes.ApiKey' property to set it."));
                eyes.ApiKey = "someAPIkey";
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("appName"));
                Configuration conf = new Configuration();
                conf.SetAppName("");
                eyes.SetConfiguration(conf);
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentException.With.Property("ParamName").EqualTo("appName"));
                conf.SetAppName("app");
                eyes.SetConfiguration(conf);
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("testName"));
                conf.SetTestName("");
                eyes.SetConfiguration(conf);
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentException.With.Property("ParamName").EqualTo("testName"));
                conf.SetTestName("test");
                eyes.SetConfiguration(conf);
                eyes.Open(driver);
            }
            finally
            {
                driver.Quit();
            }
        }
    }
}