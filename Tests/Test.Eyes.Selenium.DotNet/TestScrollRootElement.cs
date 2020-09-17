using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestScrollRootElement : TestSetup
    {
        public TestScrollRootElement(DriverOptions options) : this(options, false) { }

        public TestScrollRootElement(DriverOptions options, bool useVisualGrid)
        : base("Eyes Selenium SDK - Scroll Root Element", options, useVisualGrid)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/scrollablebody.html";
        }

        public TestScrollRootElement(DriverOptions options, StitchModes stitchMode)
        : base("Eyes Selenium SDK - Scroll Root Element", options, stitchMode)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/scrollablebody.html";
        }

        public TestScrollRootElement(string testSuitName) : base(testSuitName)
        {
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/scrollablebody.html";
        }

        [Test]
        public void TestCheckWindow_Body()
        {
            GetEyes().Check("Body (" + GetEyes().StitchMode + " stitching)", Target.Window().ScrollRootElement(By.TagName("body")).Fully());
        }

        [Test]
        public void TestCheckWindow_Html()
        {
            GetEyes().Check("Html (" + GetEyes().StitchMode + " stitching)", Target.Window().ScrollRootElement(By.TagName("html")).Fully());
        }
       
    }
}
