using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestSpecialCases : TestSetup
    {
        public TestSpecialCases(DriverOptions options) : this(options, false) { }

        public TestSpecialCases(DriverOptions options, bool useVisualGrid)
          : base("Eyes Selenium SDK - Special Cases", options, useVisualGrid)
        {
            testedPageUrl = "data:,";
        }

        public TestSpecialCases(DriverOptions options, StitchModes stitchMode)
          : base("Eyes Selenium SDK - Special Cases", options, stitchMode)
        {
            testedPageUrl = "data:,";
        }

        [Test]
        public void TestCheckRegionInAVeryBigFrame()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/WixLikeTestPage/index.html";
            GetEyes().Check("map", Target.Frame("frame1").Region(By.TagName("img")));
        }

        [Test]
        public void TestCheckRegionInAVeryBigFrameAfterManualSwitchToFrame()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/WixLikeTestPage/index.html";
            GetDriver().SwitchTo().Frame("frame1");

            //IWebElement element = driver_.FindElement(By.CssSelector("img"));
            //((IJavaScriptExecutor)driver_).ExecuteScript("arguments[0].scrollIntoView(true);", element);
            //StitchModes originalStitchMode = eyes_.StitchMode;
            //eyes_.StitchMode = StitchModes.Scroll;
            GetEyes().Check("", Target.Region(By.CssSelector("img")));
            //eyes_.StitchMode = originalStitchMode;
        }

        [Test]
        public void TestCheckElementBiggerThanHTML()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/SpecialCases/element_bigger_than_html.html";
            GetEyes().Check("nav", Target.Region(By.CssSelector("nav")));
        }
        
        [Test]
        public void TestCheckElementFullyOnBottomAfterScroll()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/TestBodyGreaterThanHtml/";
            ((IJavaScriptExecutor)GetDriver()).ExecuteScript("window.scrollTo(0, document.body.scrollHeight)");
            IWebElement element = GetDriver().FindElement(By.CssSelector("html > body > div"));
            GetEyes().Check(Target.Region(element).Fully());
        }


        [Test]
        public void TestCheckFixedRegion_Fully()
        {
            GetDriver().Url = "http://applitools.github.io/demo/TestPages/fixed-position";
            GetEyes().Check(Target.Region(By.Id("fixed")).Fully());
        }

        [Test]
        public void TestSimpleModal()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/ModalsPage/index.html";
            GetDriver().FindElement(By.Id("open_simple_modal")).Click();
            GetEyes().Check(Target.Region(By.CssSelector("#simple_modal > .modal-content")));
        }

        [Test]
        public void TestCorsIframe()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/CorsTestPage/";
            GetEyes().Check(Target.Window().Fully());
        }
    }
}
