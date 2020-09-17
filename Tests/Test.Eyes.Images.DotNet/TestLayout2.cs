namespace Applitools.Images.Tests
{
    using System.Drawing;
    using System.IO;
    using Applitools.Images;
    using Applitools.Tests.Utils;
    using Applitools.Utils;
    using NUnit.Framework;

    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestLayout2 : ReportingTestSuite
    {
        #region Fields

        private const string AppName = "Layout2";
        private BatchInfo batch_;
        private bool baseline_;

        #endregion

        #region Methods

        #region Public

        #region Setup

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            batch_ = new BatchInfo("Layout2");
        }

        public Eyes Setup()
        {
            baseline_ = false;

            Eyes eyes = new Eyes();

            eyes.BaselineEnvName = "Layout2";
            eyes.Batch = batch_;
            eyes.SetLogHandler(new StdoutLogHandler());
            eyes.MatchLevel = MatchLevel.Layout;
            if (baseline_)
            {
                eyes.SaveNewTests = true;
                eyes.SaveDiffs = true;
            }

            eyes.Logger.Log("running test: " + TestContext.CurrentContext.Test.Name);

            return eyes;
        }

        public void TearDown(Eyes eyes)
        {
            eyes.Abort();
        }

        #endregion

        #region Tests

#if false
        [Test]
        public void TestYahoo1ab()
        {
            Eyes eyes = Setup();
            eyes.Open(AppName, "Yahoo + bug", new Size(1024, 768));

            Check_("yahoo1a.png", "yahoo1b.png");
            TearDown(eyes);
        }
#endif

        //[Test]
        public void TestYahoo2ac()
        {
            Eyes eyes = Setup();
            if (baseline_)
            {
                eyes.SetAppEnvironment("Windows 6.1", "Chrome");
            }
            else
            {
                eyes.SetAppEnvironment("Windows 6.1", "IE");
            }

            eyes.Open(AppName, "Yahoo Chrome vs. IE", new Size(1024, 768));

            Check_(eyes, "yahoo2a-ie.png", "yahoo2c-chrome.png");
            TearDown(eyes);
        }

        //[Test]
        public void TestYahoo2be()
        {
            Eyes eyes = Setup();
            eyes.SetAppEnvironment("Windows 6.1", "Chrome");

            eyes.Open(AppName, "Yahoo dynamic content", new Size(1024, 768));

            Check_(eyes, "yahoo2b-chrome.png", "yahoo2e-chrome.png");
            TearDown(eyes);
        }

        [Test]
        public void TestAol1ab()
        {
            Eyes eyes = Setup();
            eyes.SetAppEnvironment("Windows 6.1", "Firefox");

            eyes.Open(AppName, "AOL dynamic content", new Size(1024, 768));

            Check_(eyes, "aol1a.png", "aol1b.png");
            TearDown(eyes);
        }

#if false
        [Test]
        public void TestGmail1()
        {
            Eyes eyes = Setup();
            eyes.Open(AppName, "Gmail dynamic content + bug", new Size(1024, 768));

            Check_("gmail1.png", "gmail2.png");
            TearDown(eyes);
        }
#endif

        //[Test]
        public void TestPaychex2()
        {
            Eyes eyes = Setup();
            if (baseline_)
            {
                eyes.SetAppEnvironment("Windows 6.1", "Chrome");
            }
            else
            {
                eyes.SetAppEnvironment("Windows 6.1", "IE");
            }

            eyes.Open(AppName, "Paychex Chrome vs. IE + bug", new Size(1024, 768));

            Check_(eyes, "paychex1a.png", "paychex1b.png");
            TearDown(eyes);
        }

#if false
        [Test]
        public void TestSnap5()
        {
            Eyes eyes = Setup();
            eyes.Open(AppName, "Snap dynamic content", new Size(1024, 768));

            Check_("snap5.png", "snap6.png");
            TearDown(eyes);
        }
#endif

        /*
        [Test]
        public void TestAmeritrade()
        {
            Eyes eyes = Setup();
            eyes.Open(AppName, "Ameritrade dynamic content", new Size(1024, 768));

            Check_("ameritrade1.png", "ameritrade2.png");
            TearDown(eyes);
        }
        */

        //[Test]
        public void TestTwitter1df()
        {
            Eyes eyes = Setup();
            if (baseline_)
            {
                eyes.SetAppEnvironment("Android", "Samsung S4");
            }
            else
            {
                eyes.SetAppEnvironment("Android", "Samsung S5");
            }

            eyes.Open(AppName, "Twitter dynamic content S4 vs S5", new Size(1024, 768));

            Check_(eyes, "twitter1d-s4.png", "twitter1f-s5.png");
            TearDown(eyes);
        }

        //[Test]
        public void TestTwitter1dg()
        {
            Eyes eyes = Setup();
            if (baseline_)
            {
                eyes.SetAppEnvironment("Android", "Samsung S4");
            }
            else
            {
                eyes.SetAppEnvironment("Android", "Samsung S5");
            }

            eyes.Open(AppName, "Twitter dynamic content S4 vs S5 + bug", new Size(1024, 768));

            Check_(eyes, "twitter1d-s4.png", "twitter1g-s5.png");
            TearDown(eyes);
        }

#if false
        [Test]
        public void TestTwitter2ab()
        {
            Eyes eyes = Setup();
            eyes.Open(AppName, "Twitter MotoG vs S3", new Size(1024, 768));

            Check_("twitter2a-motog.png", "twitter2b-s3.png");
            TearDown(eyes);
        }
#endif

        //[Test]
        public void TestDropbox1ab()
        {
            Eyes eyes = Setup();
            if (baseline_)
            {
                eyes.SetAppEnvironment("Windows 6.1", "Chrome");
            }
            else
            {
                eyes.SetAppEnvironment("Windows 6.1", "IE");
            }

            eyes.Open(AppName, "Dropbox Chrome vs. IE", new Size(1024, 768));

            Check_(eyes, "dropbox1a-chrome.png", "dropbox1b-ie.png");
            TearDown(eyes);
        }

#if false
        [Test]
        public void TestDropbox1bc()
        {
            Eyes eyes = Setup();
            eyes.Open(AppName, "Dropbox IE vs. Firefox", new Size(1024, 768));

            Check_("dropbox1b-ie.png", "dropbox1c-ff.png");
            TearDown(eyes);
        }
#endif

        #endregion

        #endregion

        #region Private

        private static string Path_(string name)
        {
            return Path.GetFullPath(Path.Combine("Content", name));
        }

        private static byte[] Bytes_(string name)
        {
            using (var r = FileUtils.GetSequentialReader(Path_(name)))
            {
                return r.ReadToEnd();
            }
        }

        private void Check_(Eyes eyes, string baseline, string actual)
        {
            if (baseline_)
            {
                eyes.CheckImageFile(Path_(baseline));
            }
            else
            {
                eyes.CheckImageFile(Path_(actual));
            }

            TestResults results = eyes.Close();
        }

        #endregion

        #endregion
    }
}
