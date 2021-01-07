using Applitools.Selenium.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class TestExceptions
    {
        [Test]
        public void TestEyesExceptions()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            EyesRunner runner = new ClassicRunner();
            Eyes eyes = new Eyes(runner);
            try
            {
                eyes.ApiKey = "";
                Assert.That(() => { eyes.Open(driver); }, Throws.Exception.With.InstanceOf<EyesException>().With.Property("Message").EqualTo("API key not set! Log in to https://applitools.com to obtain your API key and use the 'Eyes.ApiKey' property to set it."));
                eyes.ApiKey = "someAPIkey";
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("appIdOrName"));
                Configuration conf = new Configuration();
                conf.SetAppName("");
                eyes.SetConfiguration(conf);
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentException.With.Property("ParamName").EqualTo("appIdOrName"));
                conf.SetAppName("app");
                eyes.SetConfiguration(conf);
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentNullException.With.Property("ParamName").EqualTo("scenarioIdOrName"));
                conf.SetTestName("");
                eyes.SetConfiguration(conf);
                Assert.That(() => { eyes.Open(driver); }, Throws.ArgumentException.With.Property("ParamName").EqualTo("scenarioIdOrName"));
                conf.SetTestName("test");
                eyes.SetConfiguration(conf);
                eyes.Open(driver);

                TestResults results = eyes.Close(false);
                results?.Delete();
            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }

        [Test]
        public void TestEyesExceptions_VG()
        {
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            VisualGridRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            try
            {
                runner.ApiKey = null;
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
                Assert.That(() =>
                {
                    TestResults results = eyes.Close();
                    results?.Delete();
                    runner.GetAllTestResults();
                }, Throws.InvalidOperationException.With.Property("Message").EqualTo("Eyes not open"));

            }
            finally
            {
                eyes.Abort();
                driver.Quit();
            }
        }
    }
}