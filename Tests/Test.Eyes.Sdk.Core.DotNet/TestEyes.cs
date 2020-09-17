using System.Drawing;
using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    internal class TestEyes : EyesBase
    {
        private IConfiguration configuration_ = new Configuration();

        public TestEyes()
        {
        }

        protected override Configuration Configuration => (Configuration)configuration_;
        public IConfiguration GetConfiguration() => configuration_;
        public void SetConfiguration(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }

        protected override string GetInferredEnvironment()
        {
            return "TestEyes";
        }

        protected override EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            return new TestEyesScreenshot();
        }

        protected override string GetTitle()
        {
            return "TestEyes_Title";
        }

        protected override Size GetViewportSize()
        {
            return new Size(100, 100);
        }

        protected override void SetViewportSize(RectangleSize size)
        {
        }
    }
}