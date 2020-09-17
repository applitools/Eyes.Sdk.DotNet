using Applitools.Metadata;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    public class TestIEyesVG2 : TestIEyesBase
    {
        private static readonly BatchInfo batchInfo_ = new BatchInfo("Top Sites - Visual Grid 2");
        private VisualGridRunner runner_;

        public TestIEyesVG2() : base("visual_grid2")
        {
            runner_ = new VisualGridRunner(10, LogHandler);
            runner_.ServerUrl = SERVER_URL;
            runner_.ApiKey = API_KEY;
        }

        protected override Eyes InitEyes(IWebDriver webDriver, string testedUrl)
        {
            Eyes eyes = new Eyes(runner_);
            eyes.SetLogHandler(LogHandler);
            logger_ = eyes.Logger;

            eyes.Batch = batchInfo_;
            eyes.Open(webDriver, "Top Sites", "Top Sites - " + testedUrl, new System.Drawing.Size(800, 600));
            return eyes;
        }

        protected override ICheckSettings GetCheckSettings()
        {
            return base.GetCheckSettings().Fully(false);
        }

        internal override void ValidateResults(Eyes eyes, TestResults results)
        {
            SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);
            Assert.NotNull(sessionResults);

            ActualAppOutput[] actualAppOutputs = sessionResults.ActualAppOutput;
            Assert.NotNull(actualAppOutputs);

            Assert.AreEqual(2, actualAppOutputs.Length);

            ImageIdentifier image1 = actualAppOutputs[0].Image;
            Assert.IsTrue(image1.HasDom);
            Assert.AreEqual(800, image1.Size.Width);
            Assert.AreEqual(600, image1.Size.Height);

            ImageIdentifier image2 = actualAppOutputs[1].Image;
            Assert.IsTrue(image2.HasDom);
        }

    }
}
