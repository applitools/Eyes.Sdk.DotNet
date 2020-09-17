using System.Drawing;

namespace Applitools
{
    public interface IDebugScreenshotProvider
    {
        string Prefix { get; set; }

        string Path { get; set; }

        void SetLogger(Logger logger);

        void Save(Bitmap image, string suffix);
    }
}
