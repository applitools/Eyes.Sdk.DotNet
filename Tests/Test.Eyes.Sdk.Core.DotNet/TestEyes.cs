using System.Drawing;
using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    internal class TestEyes : EyesBase
    {
        private IConfiguration configuration_ = new Configuration();

        public TestEyes() : base(new ClassicRunner())
        {
        }

        protected  Configuration Configuration => (Configuration)configuration_;
        public IConfiguration GetConfiguration() => configuration_;
        protected  void SetConfigImpl(IConfiguration configuration)
        {
            configuration_ = new Configuration(configuration);
        }

        protected  string GetInferredEnvironment()
        {
            return "TestEyes";
        }

        protected  EyesScreenshot GetScreenshot(Rectangle? targetRegion, ICheckSettingsInternal checkSettingsInternal)
        {
            return new TestEyesScreenshot();
        }

        protected  string GetTitle()
        {
            return "TestEyes_Title";
        }

        protected  Size GetViewportSize()
        {
            return new Size(100, 100);
        }

        protected  void SetViewportSize(RectangleSize size)
        {
        }

        public override TestResults Abort()
        {
            throw new System.NotImplementedException();
        }
    }
}