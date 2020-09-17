using NUnit.Framework;
using System;

namespace Applitools.Windows.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestAPI
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
            StringAssert.StartsWith("Eyes.Windows.DotNet/", eyes.FullAgentId);
        }

        [Test]
        public void TestFullAgentId_2()
        {
            string serverUrlStr = "https://eyesapi.applitools.com";
            Uri serverUrl = new Uri(serverUrlStr);
            Eyes eyes = new Eyes(serverUrl);
            StringAssert.StartsWith("Eyes.Windows.DotNet/", eyes.FullAgentId);
        }

        [Test]
        public void TestFullAgentId_3()
        {
            string serverUrlStr = "https://eyesapi.applitools.com";
            Eyes eyes = new Eyes(serverUrlStr);
            StringAssert.StartsWith("Eyes.Windows.DotNet/", eyes.FullAgentId);
        }
    }
}
