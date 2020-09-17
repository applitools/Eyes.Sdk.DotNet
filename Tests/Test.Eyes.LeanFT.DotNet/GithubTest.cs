using NUnit.Framework;
using HP.LFT.SDK;
using System;
using HP.LFT.SDK.Web;

namespace Applitools.LeanFT.Tests
{
    [TestFixture]
    public class GithubTest
    {
        #region setup and teardown
        [OneTimeSetUp]
        public void Setup()
        {
            SDK.Init(new SdkConfiguration());
        }

        [OneTimeTearDown]
        public void TearDown()
        {
            SDK.Cleanup();
        }
        #endregion

        [Test]
        public void TestGithub()
        {
            var eyes = new Eyes(new Uri("https://demo.applitools.com"));
            Applitools.Tests.Utils.TestUtils.SetupLogging(eyes);
            eyes.ForceFullPageScreenshot = true;
            eyes.StitchMode = StitchModes.CSS;
            eyes.HideScrollbars = true;
            eyes.BaselineEnvName = "Desktop Web - LeanFT";
            eyes.MatchLevel = MatchLevel.Layout;

            IBrowser testBrowser = BrowserFactory.Launch(HP.LFT.SDK.Web.BrowserType.Chrome);

            try
            {
                eyes.Open(testBrowser, "Github", "Github envs", new System.Drawing.Size(800, 600));

                testBrowser.Navigate("https://www.github.com");
                eyes.CheckWindow("Search");

                eyes.Close();
            }
            finally
            {
                eyes.AbortIfNotClosed();
                testBrowser.Close();
            }
        }
    }
}
