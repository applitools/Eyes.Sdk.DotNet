using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable]
    public class TestScrollRootElementInCenter : TestSetup
    {
        public TestScrollRootElementInCenter(DriverOptions options) : this(options, false) { }

        public TestScrollRootElementInCenter(DriverOptions options, bool useVisualGrid)
        : base("Eyes Selenium SDK - Scroll Root Element", options, useVisualGrid)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithScrollableArea/index.html";
        }

        public TestScrollRootElementInCenter(DriverOptions options, StitchModes stitchMode)
        : base("Eyes Selenium SDK - Scroll Root Element", options, stitchMode)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithScrollableArea/index.html";
        }

        public TestScrollRootElementInCenter(string testSuitName) : base(testSuitName)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithScrollableArea/index.html";
        }

        [Test]
        public void TestCheckScrollRootElement()
        {
            GetEyes().Check("Scrollable area (" + GetEyes().StitchMode + " stitching)", 
                Target.Region(By.CssSelector("article")).ScrollRootElement(By.CssSelector("div.wrapper")).Fully());
        }
       
    }
}
