using NUnit.Framework;
using OpenQA.Selenium;
using System.Threading;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestClassicApi : TestSetup
    {
        public TestClassicApi(string testSuitName) : base(testSuitName) { }

        public TestClassicApi(DriverOptions options) : this(options, false) { }

        public TestClassicApi(DriverOptions options, bool useVisualGrid)
         : base("Eyes Selenium SDK - Classic API", options, useVisualGrid) { }

        public TestClassicApi(DriverOptions options, StitchModes stitchMode)
         : base("Eyes Selenium SDK - Classic API", options, stitchMode) { }

        [Test]
        public void TestCheckWindow()
        {
            GetEyes().CheckWindow("Window");
        }

        [Test]
        public void TestCheckWindow_ForceFullPageScreenshot()
        {
            bool originalForceFullPageScreenshot = GetEyes().ForceFullPageScreenshot;
            GetEyes().ForceFullPageScreenshot = true;
            GetEyes().CheckWindow("Window");
            GetEyes().ForceFullPageScreenshot = originalForceFullPageScreenshot;
        }

        [Test]
        public void TestCheckWindowFully()
        {
            GetEyes().CheckWindow("Full Window", true);
        }

        [Test]
        public void TestCheckWindowViewport()
        {
            GetEyes().CheckWindow("Viewport Window", false);
        }

        [Test]
        public void TestCheckRegion()
        {
            GetEyes().CheckRegion(By.Id("overflowing-div"), "Region", true);
        }

        [Test]
        public void TestCheckRegion2()
        {
            GetEyes().CheckRegion(By.Id("overflowing-div-image"), "minions", true);
        }

        [Test]
        public void TestCheckFrame()
        {
            GetEyes().CheckFrame("frame1", "frame1");
        }

        [Test]
        public void TestCheckRegionInFrame()
        {
            GetEyes().CheckRegionInFrame("frame1", By.Id("inner-frame-div"), "Inner frame div", true);
        }

        [Test]
        public void TestCheckInnerFrame()
        {
            GetEyes().HideScrollbars = false;
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("document.documentElement.scrollTop=350;");
            GetDriver().SwitchTo().DefaultContent();
            GetDriver().SwitchTo().Frame(GetDriver().FindElement(By.Name("frame1")));
            GetEyes().CheckFrame("frame1-1", "inner-frame");
            GetEyes().Logger.Log("Validating (1) ...");
            GetEyes().CheckWindow("window after check frame");
            GetEyes().Logger.Log("Validating (2) ...");
            IWebElement innerFrameBody = GetDriver().FindElement(By.TagName("body"));
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("arguments[0].style.background='red';", innerFrameBody);
            GetEyes().CheckWindow("window after change background color of inner frame");
        }

        [Test]
        public void TestCheckWindowAfterScroll()
        {
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("document.documentElement.scrollTop=350;");
            GetEyes().CheckWindow("viewport after scroll", false);
        }

        [Test]
        public void TestDoubleCheckWindow()
        {
            GetEyes().CheckWindow("first");
            GetEyes().CheckWindow("second");
            Thread.Sleep(3000);
        }
    }
}