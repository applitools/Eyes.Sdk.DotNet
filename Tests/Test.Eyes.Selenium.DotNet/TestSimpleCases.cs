using NUnit.Framework;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Tests
{
    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable]
    public class TestSimpleCases : TestSetup
    {
        public TestSimpleCases(DriverOptions options) : this(options, false) { }

        public TestSimpleCases(DriverOptions options, bool useVisualGrid)
        : base("Eyes Selenium SDK - Simple Test Cases", options, useVisualGrid)
        {
            testedPageSize = new Size(1024, 600);
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/";
        }

        public TestSimpleCases(DriverOptions options, StitchModes stitchMode)
              : base("Eyes Selenium SDK - Simple Test Cases", options, stitchMode)
        {
            testedPageSize = new Size(1024, 600);
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/";
        }

        public TestSimpleCases(string testSuitName) : base(testSuitName)
        {
            testedPageSize = new Size(1024, 600);
            testedPageUrl = "https://applitools.github.io/demo/TestPages/SimpleTestPage/";
        }

        [Test]
        public void TestCheckDivOverflowingThePage()
        {
            GetEyes().Check("overflowing div", Target.Region(By.Id("overflowing-div")).Fully());
        }

    }
}
