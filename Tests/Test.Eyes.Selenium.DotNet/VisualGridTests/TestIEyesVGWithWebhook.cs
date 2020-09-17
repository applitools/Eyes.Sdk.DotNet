using NUnit.Framework;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [TestFixture]
    public class TestIEyesVGWithWebhook : TestIEyesVG
    {
        private static readonly BatchInfo batchInfo_ = new BatchInfo("Top Sites - Visual Grid With Webhook");

        public TestIEyesVGWithWebhook() : base("visual_grid_with_webhook") {
        }

        protected override BatchInfo BatchInfo { get => batchInfo_; }

        protected override ICheckSettings GetCheckSettings()
        {
            string jshook = "document.body.style='background-color: red'";
            return base.GetCheckSettings().BeforeRenderScreenshotHook(jshook);
        }
    }
}
