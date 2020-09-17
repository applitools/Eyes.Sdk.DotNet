using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestPageWithHeader : TestSetup
    {
        public TestPageWithHeader(DriverOptions options) : this(options, false) { }

        public TestPageWithHeader(DriverOptions options, bool useVisualGrid)
        : base("Eyes Selenium SDK - Page With Header", options, useVisualGrid)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithHeader/index.html";
        }

        public TestPageWithHeader(DriverOptions options, StitchModes stitchMode)
        : base("Eyes Selenium SDK - Page With Header", options, stitchMode)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithHeader/index.html";
        }

        public TestPageWithHeader(string testSuitName) : base(testSuitName)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithHeader/index.html";
        }

        [Test]
        public void TestCheckPageWithHeader_Window()
        {
            GetEyes().Check("Page with header", Target.Window().Fully(false));
        }

        [Test]
        public void TestCheckPageWithHeader_Window_Fully()
        {
            GetEyes().Check("Page with header - fully", Target.Window().Fully(true));
        }

        [Test]
        public void TestCheckPageWithHeader_Region()
        {
            GetEyes().Check("Page with header", Target.Region(By.CssSelector("div.page")).Fully(false));
        }

        [Test]
        public void TestCheckPageWithHeader_Region_Fully()
        {
            GetEyes().Check("Page with header - fully", Target.Region(By.CssSelector("div.page")).Fully(true));
        }

    }
}
