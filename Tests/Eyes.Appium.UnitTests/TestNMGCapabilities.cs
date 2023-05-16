using System;
using Applitools;
using NUnit.Framework;
using OpenQA.Selenium.Appium;

namespace Eyes.Appium.UnitTests
{
    public class TestNmgCapabilities
    {
        private const string API_KEY = "asd123";
        private const string SERVER_URL = "https://eyesapi.applitools.com";
        private static ProxySettings proxySettings = new ProxySettings("http://127.0.0.1", 8888);

        private const string LIBRARY_PATH =
            "@executable_path/Frameworks/UFG_lib.xcframework/ios-arm64/UFG_lib.framework/UFG_lib:@executable_path/Frameworks/UFG_lib.xcframework/ios-arm64_x86_64-simulator/UFG_lib.framework/UFG_lib";

        [Test]
        public void TestAndroidNmgCaps()
        {
            var caps = GetCaps();

            Applitools.Appium.Eyes.SetNmgCapabilities(caps, API_KEY, SERVER_URL, proxySettings);
            
            caps.ToDictionary().TryGetValue("optionalIntentArguments", out object androidArgs);
            Assert.NotNull(androidArgs);
            Assert.IsInstanceOf<string>(androidArgs);

            string[] androidIntentArguments = ((string)androidArgs).Split(" ", 3);
            Assert.AreEqual(androidIntentArguments[0], "--es");
            Assert.AreEqual(androidIntentArguments[1], "APPLITOOLS");
            Assert.AreEqual(androidIntentArguments[2],
                $"'{{\"NML_API_KEY\":\"{API_KEY}\",\"NML_SERVER_URL\":\"{SERVER_URL}\",\"NML_PROXY_URL\":\"{proxySettings}\",}}'");
        }

        [Test]
        public void TestIosNmgCaps()
        {
            var caps = GetCaps();

            Applitools.Appium.Eyes.SetNmgCapabilities(caps, API_KEY, SERVER_URL, proxySettings);

            caps.ToDictionary().TryGetValue("processArguments", out object iosArgs);
            Assert.NotNull(iosArgs);
            Assert.IsInstanceOf<string>(iosArgs);

            string[] iosIntentArguments = ((string)iosArgs).Split(" ", 3);
            Assert.AreEqual(iosIntentArguments[0], "{\"args\":");
            Assert.AreEqual(iosIntentArguments[1], "[],");
            Assert.AreEqual(iosIntentArguments[2],
                $"\"env\":{{\"DYLD_INSERT_LIBRARIES\":\"{LIBRARY_PATH}\",\"NML_API_KEY\":\"{API_KEY}\",\"NML_SERVER_URL\":\"{SERVER_URL}\",\"NML_PROXY_URL\":\"{proxySettings}\",}}}}");
        }

        [Test]
        public void TestDefaultsFromEnv()
        {
            var caps = GetCaps();

            const string FAKE_API_KEY = "MY_API_KEY_123";
            const string FAKE_SERVER_URL = "applitools-server!";
            const string FAKE_PROXY_URI = "user:pass@myproxy.applitools.com";

            Environment.SetEnvironmentVariable("APPLITOOLS_API_KEY", FAKE_API_KEY);
            Environment.SetEnvironmentVariable("APPLITOOLS_SERVER_URL", FAKE_SERVER_URL);
            Environment.SetEnvironmentVariable("APPLITOOLS_HTTP_PROXY", FAKE_PROXY_URI);

            Applitools.Appium.Eyes.SetNmgCapabilities(caps);
            
            caps.ToDictionary().TryGetValue("optionalIntentArguments", out object androidArgs);
            Assert.NotNull(androidArgs);
            Assert.IsInstanceOf<string>(androidArgs);

            string[] androidIntentArguments = ((string)androidArgs).Split(" ", 3);
            Assert.AreEqual(androidIntentArguments[0], "--es");
            Assert.AreEqual(androidIntentArguments[1], "APPLITOOLS");
            Assert.AreEqual(androidIntentArguments[2],
                $"'{{\"NML_API_KEY\":\"{FAKE_API_KEY}\",\"NML_SERVER_URL\":\"{FAKE_SERVER_URL}\",\"NML_PROXY_URL\":\"{FAKE_PROXY_URI}\",}}'");
        }

        private AppiumOptions GetCaps()
        {
            var caps = new AppiumOptions();
            caps.AddAdditionalCapability("deviceName", "Google Pixel 5 GoogleAPI Emulator");
            caps.AddAdditionalCapability("deviceOrientation", "portrait");
            caps.AddAdditionalCapability("platformVersion", "11.0");
            caps.AddAdditionalCapability("platformName", "Android");
            caps.AddAdditionalCapability("automationName", "UiAutomator2");

            return caps;
        }
    }
}
