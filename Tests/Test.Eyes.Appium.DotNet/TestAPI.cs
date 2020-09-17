using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium.Appium;
using OpenQA.Selenium.Remote;
using System;

namespace Applitools.Appium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestAPI : ReportingTestSuite
    {
        [Test]
        public void EnsureApiExists()
        {
            Eyes eyes = new Eyes();
            Configuration config = eyes.GetConfiguration();
            Assert.NotNull(config);
            config.SetAppName("Test API").SetTestName("Ensure API Exists");
            eyes.SetConfiguration(config);
            Assert.AreEqual("Test API", eyes.AppName);
            Assert.AreEqual("Ensure API Exists", eyes.TestName);
        }

        [Test]
        public void TestIsDisabled()
        {
            Eyes eyes = new Eyes();
            TestUtils.SetupLogging(eyes);
            eyes.IsDisabled = true;
            AppiumDriver<AppiumWebElement> driver = null;
            eyes.Open(driver, "Test", "Test");
            eyes.CheckWindow("Test Window");
            eyes.Close();
        }

        [Test]
        public void TestEyesConstructor_PassServerUrl()
        {
            string serverUrlStr = "https://eyesapi.applitools.com";
            Uri serverUrl = new Uri(serverUrlStr);
            Eyes eyes = new Eyes(serverUrl);
            Assert.AreEqual(serverUrl.ToString(), eyes.ServerUrl);
        }


        [Test]
        public void TestFullAgentId_1()
        {
            Eyes eyes = new Eyes();
            StringAssert.StartsWith("Eyes.Appium.DotNet/", eyes.FullAgentId);
        }

        [Test]
        public void TestFullAgentId_2()
        {
            string serverUrlStr = "https://eyesapi.applitools.com";
            Uri serverUrl = new Uri(serverUrlStr);
            Eyes eyes = new Eyes(serverUrl);
            StringAssert.StartsWith("Eyes.Appium.DotNet/", eyes.FullAgentId);
        }
    }
}
