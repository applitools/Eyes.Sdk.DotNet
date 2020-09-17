using System.Drawing;

namespace Applitools
{
    public class NullDebugScreenshotProvider : IDebugScreenshotProvider
    {
        private static NullDebugScreenshotProvider instance_;

        public string Path { get; set; }

        public string Prefix { get; set; }

        public void Save(Bitmap image, string suffix) { }

        public void SetLogger(Logger logger) { }

        public static NullDebugScreenshotProvider Instance
        {
            get
            {
                if (instance_ == null)
                {
                    instance_ = new NullDebugScreenshotProvider();
                }
                return instance_;
            }
        }
    }
}
