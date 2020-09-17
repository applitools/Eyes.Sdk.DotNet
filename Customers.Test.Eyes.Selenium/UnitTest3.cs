using Applitools;
using Applitools.Metadata;
using Applitools.Selenium;
using Applitools.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
using System.Runtime.CompilerServices;
using System.Web;

namespace Elad
{
    [TestFixture]
    [Parallelizable(ParallelScope.Children)]
    public class UnitTest3
    {
        [Test]
        public void TestConfig_ForceFullPageScreenshot_VG()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);

            DoTestConfig_ForceFullPageScreenshot_(eyes);
        }

        [Test]
        public void TestConfig_ForceFullPageScreenshot_Selenium()
        {
            Eyes eyes = new Eyes();
            DoTestConfig_ForceFullPageScreenshot_(eyes);
        }

        [Test]
        public void TestConfig_MatchLevel_VG()
        {
            EyesRunner runner = new VisualGridRunner(10);
            Eyes eyes = new Eyes(runner);
            DoTestConfig_MatchLevel_(eyes);
        }

        [Test]
        public void TestConfig_MatchLevel_Selenium()
        {
            Eyes eyes = new Eyes();
            DoTestConfig_MatchLevel_(eyes);
        }

        private static void DoTestConfig_ForceFullPageScreenshot_(Eyes eyes, [CallerMemberName] string testName = null)
        {
            IWebDriver webDriver = new ChromeDriver();
            try
            {
                webDriver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";

                eyes.ForceFullPageScreenshot = true;
                eyes.Open(webDriver, "test", testName, new System.Drawing.Size(1000, 700));
                eyes.Check(Target.Window());

                eyes.ForceFullPageScreenshot = false;
                eyes.Check(Target.Window());

                eyes.Close();
            }
            finally
            {
                webDriver.Quit();
            }
        }

        private static void DoTestConfig_MatchLevel_(Eyes eyes, [CallerMemberName] string testName = null)
        {
            IWebDriver webDriver = new ChromeDriver();
            try
            {
                webDriver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";

                eyes.MatchLevel = MatchLevel.Layout;
                eyes.Open(webDriver, "test", testName, new System.Drawing.Size(1000, 700));
                eyes.Check(Target.Window());

                eyes.MatchLevel = MatchLevel.Content;
                eyes.Check(Target.Window());
            }
            finally
            {
                webDriver.Quit();
            }

            TestResults results = eyes.Close();
            SessionResults sessionResults = GetSessionResults_(eyes, results);

            Assert.NotNull(sessionResults);
            Assert.NotNull(sessionResults.ActualAppOutput);
            Assert.AreEqual(2, sessionResults.ActualAppOutput.Length);
            Assert.AreEqual(MatchLevel.Layout2, sessionResults.ActualAppOutput[0].ImageMatchSettings.MatchLevel);
            Assert.AreEqual(MatchLevel.Content, sessionResults.ActualAppOutput[1].ImageMatchSettings.MatchLevel);
        }

        private static SessionResults GetSessionResults_(Eyes eyes, TestResults results)
        {
            string apiSessionUrl = results.ApiUrls?.Session;

            SessionResults sessionResults = null;
            if (apiSessionUrl != null)
            {
                var uriBuilder = new UriBuilder(apiSessionUrl);
                var query = HttpUtility.ParseQueryString(uriBuilder.Query);
                query["format"] = "json";
                query["AccessToken"] = results.SecretToken;
                query["apiKey"] = eyes.ApiKey;
                uriBuilder.Query = query.ToString();

                HttpRestClient client = new HttpRestClient(uriBuilder.Uri);
                var metaResults = client.Get(uriBuilder.ToString());
                sessionResults = metaResults.DeserializeBody<SessionResults>(false);
            }
            return sessionResults;
        }
    }
}
