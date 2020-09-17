using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable]
    public class TestScrollableAreaInCenter : TestSetup
    {
        public TestScrollableAreaInCenter(DriverOptions options) : this(options, false) { }

        public TestScrollableAreaInCenter(DriverOptions options, bool useVisualGrid)
        : base("Eyes Selenium SDK", options, useVisualGrid)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithScrollableArea/index2.html";
        }

        public TestScrollableAreaInCenter(DriverOptions options, StitchModes stitchMode)
        : base("Eyes Selenium SDK", options, stitchMode)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithScrollableArea/index2.html";
        }

        public TestScrollableAreaInCenter(string testSuitName) : base(testSuitName)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/PageWithScrollableArea/index2.html";
        }

        [Test]
        public void TestCheckScrollableAreaInCenter()
        {
            By article = By.CssSelector("article");
            GetEyes().Check("Scrollable area (" + GetEyes().StitchMode + " stitching)", Target.Region(article).ScrollRootElement(article).Fully());
        }
       
    }
}
