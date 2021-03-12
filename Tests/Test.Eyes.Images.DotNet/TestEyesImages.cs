using System;
using System.Drawing;
using System.IO;
using System.Reflection;
using Applitools.Tests.Utils;
using Applitools.Utils;
using NUnit.Framework;

namespace Applitools.Images.Tests
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestEyesImages : ReportingTestSuite
    {
        private BatchInfo batch_;

        [OneTimeSetUp]
        public void FixtureSetUp()
        {
            batch_ = new BatchInfo("TestEyesImages");
        }

        public Eyes Setup()
        {
            Eyes eyes = new Eyes();
            eyes.Batch = batch_;
            string testName = TestContext.CurrentContext.Test.Name;
            TestUtils.SetupLogging(eyes, testName);
            return eyes;
        }

        public TestResults Teardown(Eyes eyes)
        {
            try
            {
                TestResults results = eyes.Close();
                eyes.Logger.Log("Mismatches: " + results.Mismatches);
                return results;
            }
            finally
            {
                eyes.Abort();
            }
        }

        [Test]
        public void TestBitmap()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage(Bitmap)");

            eyes.CheckImage(new Bitmap(CommonUtils.ReadResourceStream("Test.Eyes.Images.DotNet.Content.gbg1.png")), "TestBitmap1");
            eyes.CheckImage(new Bitmap(CommonUtils.ReadResourceStream("Test.Eyes.Images.DotNet.Content.gbg2.png")), "TestBitmap2");
            Teardown(eyes);
        }

        [Test]
        public void TestBitmapWithDispose()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage(Bitmap) With Dispose");

            using (Bitmap bitmap1 = new Bitmap(CommonUtils.ReadResourceStream("Test.Eyes.Images.DotNet.Content.gbg1.png")))
            {
                eyes.CheckImage(bitmap1, "TestBitmap1");
            }

            using (Bitmap bitmap2 = new Bitmap(CommonUtils.ReadResourceStream("Test.Eyes.Images.DotNet.Content.gbg2.png")))
            {
                eyes.CheckImage(bitmap2, "TestBitmap2");
            }

            Teardown(eyes);
        }

        [Test]
        public void TestBytes()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage(byte[])", new Size(1024, 768));

            eyes.CheckImage(ReadResourceBytes_("gbg1.png"), "TestBytes1");
            eyes.CheckImage(ReadResourceBytes_("gbg2.png"), "TestBytes2");
            Teardown(eyes);
        }

        [Test]
        public void TestBase64()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage(base64)", new Size(1024, 768));

            eyes.CheckImage(Convert.ToBase64String(ReadResourceBytes_("gbg1.png")), "TestBase64 1");
            eyes.CheckImage(Convert.ToBase64String(ReadResourceBytes_("gbg2.png")), "TestBase64 2");
            Teardown(eyes);
        }

        [Test]
        public void TestFile()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImageFile", new Size(1024, 768));

            eyes.CheckImageFile(Path_("gbg1.png"), "TestPath1");
            eyes.CheckImageFile(Path_("gbg2.png"), "TestPath2");
            Teardown(eyes);
        }

        [Test]
        public void TestUrl()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImageAtUrl", new Size(1024, 768));

            eyes.CheckImageAtUrl(new Uri("https://applitools.github.io/demo/images/gbg1.png").AbsoluteUri, "TestUrl1");
            eyes.CheckImageAtUrl(new Uri("https://applitools.github.io/demo/images/gbg2.png").AbsoluteUri, "TestUrl2");
            Teardown(eyes);
        }

        [Test]
        public void TestFluent_Path()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage_Fluent", new Size(1024, 768));
            eyes.Check("CheckImage_Fluent", Target.Image(Path_("gbg1.png")));
            Teardown(eyes);
        }

        [Test]
        public void TestFluent_Bitmap()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage_Fluent", new Size(1024, 768));
            eyes.Check("CheckImage_Fluent", Target.Image(new Bitmap(CommonUtils.ReadResourceStream("Test.Eyes.Images.DotNet.Content.gbg1.png"))));
            Teardown(eyes);
        }

        [Test]
        public void TestFluent_WithIgnoreRegion()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage_WithIgnoreRegion_Fluent", new Size(1024, 768));
            eyes.Check("CheckImage_WithIgnoreRegion_Fluent", Target.Image(Path_("gbg1.png")).Ignore(new Rectangle(10, 20, 30, 40)));
            TestResults testResults = Teardown(eyes);
            var sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, testResults);
            Assert.IsNotNull(sessionResults.ActualAppOutput);
            Assert.AreEqual(1, sessionResults.ActualAppOutput.Length);
            Assert.IsNotNull(sessionResults.ActualAppOutput[0].ImageMatchSettings.Ignore);
            Assert.AreEqual(1, sessionResults.ActualAppOutput[0].ImageMatchSettings.Ignore.Length);
            Assert.AreEqual(new Utils.Geometry.Region(10, 20, 30, 40), sessionResults.ActualAppOutput[0].ImageMatchSettings.Ignore[0]);
        }

        [Test]
        public void TestFluent_WithRegion()
        {
            Eyes eyes = Setup();
            eyes.Open("TestEyesImages", "CheckImage_WithRegion_Fluent");
            eyes.Check("CheckImage_WithRegion_Fluent", Target.Image(Path_("gbg1.png")).Region(new Rectangle(10, 20, 30, 40)));
            Teardown(eyes);
        }

        [Test]
        public void TestLayout2()
        {
            Eyes eyes = Setup();
            eyes.MatchLevel = MatchLevel.Layout;
            eyes.Open("CheckLayout2", "Check Layout2", new Size(1024, 768));

            var path = Path_("yahoo1a.png");
            //var path = Path_("yahoo1b.png");

            //var path = Path_("aol1.png");
            //var path = Path_("aol2.png");

            eyes.CheckImageFile(path, path);
            Teardown(eyes);
        }

        [Test]
        public void TestReplaceLast()
        {
            Eyes eyes = Setup();
            eyes.Open(nameof(TestReplaceLast), nameof(TestReplaceLast));

            var path1 = Path_("paychex1a.png");
            var path2 = Path_("dropbox1a-chrome.png");

            eyes.Check(Target.Image(path1).WithName("step 1"));
            eyes.Check(Target.Image(path2).WithName("step 2"));
            eyes.Check(Target.Image(path1).WithName("step 3"));
            eyes.Check(Target.Image(path2).WithName("step 3 (replaced)").ReplaceLast());
            TestResults result = Teardown(eyes);
            Assert.AreEqual(3, result.Steps);
            Assert.AreEqual("step 1", result.StepsInfo[0].Name);
            Assert.AreEqual("step 2", result.StepsInfo[1].Name);
            Assert.AreEqual("step 3 (replaced)", result.StepsInfo[2].Name);
        }

        [Test]
        public void TestConfiguration()
        {
            Configuration config = new Configuration();

            Eyes e1 = new Eyes();
            e1.SetConfiguration(config.SetAppName("app1"));

            Eyes e2 = new Eyes();
            e2.SetConfiguration(config.SetAppName("app2"));

            Eyes e3 = new Eyes();
            config.AppName = "DefaultAppName";
            e3.SetConfiguration(config);

            Assert.AreEqual("app1", e1.GetConfiguration().AppName, "e1 app name");
            Assert.AreEqual("app2", e2.GetConfiguration().AppName, "e2 app name");
            Assert.AreEqual("DefaultAppName", e3.GetConfiguration().AppName, "e3 app name");
        }

        #region Private

        internal static string Path_(string name)
        {
            return Path.GetFullPath(Path.Combine("Content", name));
        }

        internal static byte[] Bytes_(string name)
        {
            using (var r = FileUtils.GetSequentialReader(Path_(name)))
            {
                return r.ReadToEnd();
            }
        }

        internal static Stream ReadResourceStream_(string filename)
        {
            Assembly thisAssembly = Assembly.GetCallingAssembly();
            return thisAssembly.GetManifestResourceStream("Test.Eyes.Images.DotNet.Content." + filename);
        }

        internal static byte[] ReadResourceBytes_(string filename)
        {
            return CommonUtils.ReadResourceBytes("Test.Eyes.Images.DotNet.Content." + filename);
        }

        #endregion
    }
}
