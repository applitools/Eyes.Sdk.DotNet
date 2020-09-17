using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable]
    public class TestScrollRootElementOnSimplePage : TestSetup
    {
        public TestScrollRootElementOnSimplePage(DriverOptions options) : this(options, false) { }

        public TestScrollRootElementOnSimplePage(DriverOptions options, bool useVisualGrid)
        : base("Eyes Selenium SDK - Scroll Root Element", options, useVisualGrid)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/index.html";
        }

        public TestScrollRootElementOnSimplePage(DriverOptions options, StitchModes stitchMode)
            : base("Eyes Selenium SDK - Scroll Root Element", options, stitchMode)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/index.html";
        }

        public TestScrollRootElementOnSimplePage(string testSuitName) : base(testSuitName)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/index.html";
        }

        [Test]
        public void TestCheckWindow_Simple_Html()
        {
            GetEyes().Check("Html (" + GetEyes().StitchMode + " stitching)", Target.Window().ScrollRootElement(By.TagName("html")).Fully());
        }
    }
}
