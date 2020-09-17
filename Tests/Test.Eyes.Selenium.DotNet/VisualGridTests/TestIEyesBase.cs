using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable(ParallelScope.All)]
    public abstract class TestIEyesBase : ReportingTestSuite
    {
        protected Logger logger_;

        protected readonly string SERVER_URL = "https://eyesapi.applitools.com/";
        protected readonly string API_KEY = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY");
        //protected readonly string SERVER_URL = "https://testeyes.applitools.com/";
        //protected readonly string API_KEY = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY_TESTEYES");

        public static object[] MethodArgs = new object[] {
            //new object[] { "https://google.com", MatchLevel.Strict },
            //new object[] {"https://facebook.com", MatchLevel.Strict },
            //new object[] {"https://youtube.com", MatchLevel.Layout },
            //new object[] {"https://amazon.com" , MatchLevel.Layout},
            //new object[] {"https://twitter.com", MatchLevel.Strict },
            //new object[] {"https://ebay.com", MatchLevel.Layout },
            //new object[] {"https://wikipedia.org", MatchLevel.Strict },
            //new object[] {"https://instagram.com", MatchLevel.Strict },
            //new object[] {"https://www.target.com/c/blankets-throws/-/N-d6wsb?lnk=ThrowsBlankets%E2%80%9C,tc", MatchLevel.Strict },
            //new object[] {"https://www.usatoday.com", MatchLevel.Layout },
            //new object[] {"https://www.vans.com", // TODO - this website get the flow to stuck in an endless loop.
            //new object[] {"https://docs.microsoft.com/en-us/", MatchLevel.Strict},
            //new object[] {"https://applitools.com/features/frontend-development", MatchLevel.Strict },
            //new object[] {"https://applitools.com/docs/topics/overview.html", MatchLevel.Strict },
            //new object[] { "https://www.qq.com/", MatchLevel.Strict },
        };

        protected TestIEyesBase(string fixtureName)
        {
            LogPath = TestUtils.InitLogPath(fixtureName);
            LogHandler = TestUtils.InitLogHandler(fixtureName, LogPath);
            RenderingTask.pollTimeout_ = TimeSpan.FromMinutes(2);
            VisualGridRunner.waitForResultTimeout_ = TimeSpan.FromMinutes(1);
            suiteArgs_.Add("fixture", fixtureName);
        }

        public string LogPath { get; }
        public ILogHandler LogHandler { get; }

        [TestCaseSource(nameof(MethodArgs)), Parallelizable(ParallelScope.All)]
        public void TestEyesDifferentRunners(string testedUrl, MatchLevel matchLevel)
        {
            IWebDriver webDriver = SeleniumUtils.CreateChromeDriver();
            Logger logger = null;
            Eyes eyes = null;
            try
            {
                webDriver.Url = testedUrl;
                eyes = InitEyes(webDriver, testedUrl);
                eyes.SaveNewTests = false;
                logger = eyes.Logger;
                logger.Log("running check for url " + testedUrl);
                ICheckSettings checkSettings = GetCheckSettings();
                eyes.MatchLevel = matchLevel;
                eyes.Check(checkSettings.WithName("Step1 - " + testedUrl));
                eyes.Check(checkSettings.Fully().WithName("Step2 - " + testedUrl));
                logger.Verbose("calling eyes_.Close() (test: {0})", testedUrl);
                TestResults results = eyes.Close(false);
                ValidateResults(eyes, results);
            }
            finally
            {
                eyes?.Abort();
                webDriver.Quit();
            }
        }

        internal abstract void ValidateResults(Eyes eyes, TestResults results);

        protected virtual ICheckSettings GetCheckSettings()
        {
            return Target.Window();
        }

        protected abstract Eyes InitEyes(IWebDriver webDriver, string testedUrl);
    }
}
