using Applitools.Metadata;
using Applitools.Selenium.Capture;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestSendDom : ReportingTestSuite
    {
        private static void CaptureDom(string url, Action<IWebDriver> initCode = null, [CallerMemberName] string testName = null)
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = url;
            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler(testName));
            initCode?.Invoke(webDriver);
            Eyes eyes = new Eyes();
            try
            {
                eyes.Batch = TestDataProvider.BatchInfo;
                eyes.AppName = "Test Send DOM";
                eyes.TestName = testName;
                EyesWebDriver eyesWebDriver = (EyesWebDriver)eyes.Open(webDriver);
                //DomCapture domCapture = new DomCapture(logger, eyesWebDriver);
                eyes.CheckWindow();
                TestResults results = eyes.Close(false);
                bool hasDom = GetHasDom_(eyes, results);
                Assert.IsTrue(hasDom);
                //string actualDomJsonString = domCapture.GetFullWindowDom();
                //WriteDomJson(logger, actualDomJsonString);
            }
            catch (Exception ex)
            {
                logger.Log("Error: " + ex);
                throw;
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }

        class DomInterceptingEyes : SeleniumEyes
        {
            public DomInterceptingEyes() : base(new ConfigurationProviderForTesting(), new ClassicRunner())
            {

            }
            public string DomJson { get; private set; }
            protected override string TryCaptureDom()
            {
                DomJson = base.TryCaptureDom();
                return DomJson;
            }
        }

        [Test]
        public void TestSendDOM_FullWindow()
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";
            DomInterceptingEyes eyes = new DomInterceptingEyes();
            eyes.Batch = TestDataProvider.BatchInfo;

            EyesWebDriver eyesWebDriver = (EyesWebDriver)eyes.Open(webDriver, "Test Send DOM", "Full Window", new Size(1024, 768));
            try
            {
                eyes.Check(Target.Window().Fully().WithName("Window"));
                string actualDomJsonString = eyes.DomJson;

                string expectedDomJsonString = CommonUtils.ReadResourceFile("Test.Eyes.Selenium.DotNet.Resources.expected_dom1.json");
                string expectedDomJson= JsonUtility.NormalizeJsonString(expectedDomJsonString);

                TestResults results = eyes.Close(false);
                bool hasDom = GetHasDom_(eyes, results);
                Assert.IsTrue(hasDom);
                
                string actualDomJson = JsonUtility.NormalizeJsonString(actualDomJsonString);
                Assert.AreEqual(expectedDomJson, actualDomJson);

                SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);
                ActualAppOutput[] actualAppOutput = sessionResults.ActualAppOutput;
                string downloadedDomJsonString = TestUtils.GetStepDom(eyes, actualAppOutput[0]);
                string downloadedDomJson = JsonUtility.NormalizeJsonString(downloadedDomJsonString);
                Assert.AreEqual(expectedDomJson, downloadedDomJson);
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }

        private static bool GetHasDom_(IEyesBase eyes, TestResults results)
        {
            SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);
            ActualAppOutput[] actualAppOutputs = sessionResults.ActualAppOutput;
            Assert.AreEqual(1, actualAppOutputs.Length);
            bool hasDom = actualAppOutputs[0].Image.HasDom;
            return hasDom;
        }

        //[Test]
        public void TestSendDOM_Simple_HTML()
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools-dom-capture-origin-1.surge.sh/test.html";
            Eyes eyes = new Eyes();
            try
            {
                EyesWebDriver eyesWebDriver = (EyesWebDriver)eyes.Open(webDriver, "Test Send DOM", "Test DomCapture method", new Size(1200, 1000));
                Logger logger = new Logger();
                logger.SetLogHandler(TestUtils.InitLogHandler());
                UserAgent ua = eyes.seleniumEyes_.userAgent_;
                DomCapture domCapture = new DomCapture(logger, eyesWebDriver, ua);
                string actualDomJsonString = domCapture.GetFullWindowDom();
                string actualDomJson = JsonUtility.NormalizeJsonString(actualDomJsonString);
                string expectedDomJson = GetExpectedDomFromUrl_("https://applitools-dom-capture-origin-1.surge.sh/test.dom.json");

                eyes.Close(false);

                Assert.AreEqual(expectedDomJson, actualDomJson);
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }

        [Test]
        public void TestSendDOM_Selector()
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.github.io/demo/TestPages/DomTest/dom_capture.html";
            Eyes eyes = new Eyes();
            eyes.Batch = TestDataProvider.BatchInfo;
            try
            {
                eyes.Open(webDriver, "Test SendDom", "Test SendDom", new Size(1000, 700));
                eyes.Check("region", Target.Region(By.CssSelector("#scroll1")));
                TestResults results = eyes.Close(false);
                bool hasDom = GetHasDom_(eyes, results);
                Assert.IsTrue(hasDom);
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }

        [Test]
        public void TestNotSendDOM()
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            webDriver.Url = "https://applitools.com/helloworld";
            Eyes eyes = new Eyes();
            eyes.Batch = TestDataProvider.BatchInfo;
            eyes.SetLogHandler(TestUtils.InitLogHandler());
            eyes.SendDom = false;
            try
            {
                eyes.Open(webDriver, "Test NOT SendDom", "Test NOT SendDom", new Size(1000, 700));
                eyes.Check("window", Target.Window().SendDom(false));
                TestResults results = eyes.Close(false);
                bool hasDom = GetHasDom_(eyes, results);
                Assert.IsFalse(hasDom);
            }
            finally
            {
                eyes.Abort();
                webDriver.Quit();
            }
        }

        [Test]
        public void TestSendDOM_1()
        {
            CaptureDom("https://applitools.github.io/demo/TestPages/DomTest/dom_capture.html");
        }

        [Test]
        public void TestSendDOM_2()
        {
            CaptureDom("https://applitools.github.io/demo/TestPages/DomTest/dom_capture_2.html");
        }


        private string GetExpectedDomFromUrl_(string domUrl)
        {
            HttpClient client = new HttpClient();
            string expectedDomJsonString = client.GetStringAsync(domUrl).Result;
            return JsonUtility.NormalizeJsonString(expectedDomJsonString);
        }

        private static void WriteDomJson(Logger logger, string domJson)
        {
            if (logger.GetILogHandler() is FileLogHandler fileLogger)
            {
                string path = Path.GetDirectoryName(fileLogger.FilePath);
                File.WriteAllText(Path.Combine(path, "dom.json"), domJson);
            }
        }

        public static class JsonUtility
        {
            public static string NormalizeJsonString(string json)
            {
                JToken parsed = JToken.Parse(json);

                JToken normalized = NormalizeToken(parsed);

                return JsonConvert.SerializeObject(normalized, Formatting.Indented);
            }

            private static JToken NormalizeToken(JToken token)
            {
                JObject o;
                JArray array;
                if ((o = token as JObject) != null)
                {
                    List<JProperty> orderedProperties = new List<JProperty>(o.Properties());
                    orderedProperties.Sort(delegate (JProperty x, JProperty y) { return x.Name.CompareTo(y.Name); });
                    JObject normalized = new JObject();
                    foreach (JProperty property in orderedProperties)
                    {
                        normalized.Add(property.Name, NormalizeToken(property.Value));
                    }
                    return normalized;
                }
                else if ((array = token as JArray) != null)
                {
                    for (int i = 0; i < array.Count; i++)
                    {
                        array[i] = NormalizeToken(array[i]);
                    }
                    return array;
                }
                else
                {
                    return token;
                }
            }
        }
    }
}
