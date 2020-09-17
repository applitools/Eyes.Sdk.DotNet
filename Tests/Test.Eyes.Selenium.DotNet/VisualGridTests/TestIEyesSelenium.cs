using Applitools.Metadata;
using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using NUnit.Framework;
using OpenQA.Selenium;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    public class TestIEyesSelenium : TestIEyesBase
    {
        private static readonly BatchInfo batchInfo_ = new BatchInfo("Top Sites - Selenium");

        public TestIEyesSelenium() : base("selenium")
        {
        }

        protected override Eyes InitEyes(IWebDriver webDriver, string testedUrl)
        {
            Eyes eyes = new Eyes();
            eyes.ServerUrl = SERVER_URL;
            eyes.SetLogHandler(LogHandler);
            logger_ = eyes.Logger;

            eyes.Batch = batchInfo_;
            eyes.Open(webDriver, "Top Sites", "Top Sites - " + testedUrl, new System.Drawing.Size(1024, 768));
            return eyes;
        }

        internal override void ValidateResults(Eyes eyes, TestResults results)
        {
            SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);

            ActualAppOutput[] actualAppOutputs = sessionResults.ActualAppOutput;
            Assert.AreEqual(2, actualAppOutputs.Length);

            ImageIdentifier image1 = actualAppOutputs[0].Image;
            Assert.IsTrue(image1.HasDom);
            Assert.AreEqual(1024, image1.Size.Width);
            Assert.AreEqual(768, image1.Size.Height);

            ImageIdentifier image2 = actualAppOutputs[1].Image;
            Assert.IsTrue(image2.HasDom);
        }
    }
}
