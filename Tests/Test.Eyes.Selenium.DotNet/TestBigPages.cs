using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable(ParallelScope.All)]
    public class TestBigPages : TestSetup
    {
        public TestBigPages(string testSuitName) : base(testSuitName)
        {
            testedPageUrl = "data:,";
        }

        public TestBigPages(DriverOptions options) : this(options, false) { }

        public TestBigPages(DriverOptions options, bool useVisualGrid)
            : base("Eyes Selenium SDK - Test Big Pages", options, useVisualGrid)
        {
            testedPageUrl = "data:,";
        }

        public TestBigPages(DriverOptions options, StitchModes stitchMode)
            : base("Eyes Selenium SDK - Test Big Pages", options, stitchMode)
        {
            testedPageUrl = "data:,";
        }

        [Test]
        public void TestCheckCoerceLongImage()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/SimpleTestPage/?long";
            GetEyes().Check("Cropped long image", Target.Region(By.Id("overflowing-div")).Fully());
        }

        [Test]
        public void TestCheckCoerceLargeImage()
        {
            GetDriver().Url = "https://applitools.github.io/demo/TestPages/SimpleTestPage/?large";
            GetEyes().Check("Cropped large image", Target.Region(By.Id("overflowing-div")).Fully());
        }
    }
}