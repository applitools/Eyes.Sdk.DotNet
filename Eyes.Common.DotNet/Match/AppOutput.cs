using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using System.Drawing.Imaging;
using System.IO;

namespace Applitools
{
    /// <summary>
    /// An application output.
    /// </summary>
    public sealed class AppOutput
    {
        public AppOutput(string title, Location location, IEyesScreenshot screenshot, string domUrl = null, RectangleSize viewport = null)
        {
            Title = title;
            Location = location;
            ScreenshotUrl = null;
            DomUrl = domUrl;
            Viewport = viewport;
            Screenshot = screenshot;
            using (MemoryStream stream = new MemoryStream())
            {
                screenshot.Image.Save(stream, ImageFormat.Png);
                ScreenshotBytes = stream.ToArray();
            }
        }

        public AppOutput(string title, Location location, byte[] screenshotBytes, string screenshotUrl, string domUrl = null, RectangleSize viewport = null)
        {
            Title = title;
            Location = location;
            ScreenshotBytes = screenshotBytes;
            ScreenshotUrl = screenshotUrl;
            DomUrl = domUrl;
            Viewport = viewport;
        }

        [JsonIgnore]
        public byte[] ScreenshotBytes { get; }

        [JsonIgnore]
        public IEyesScreenshot Screenshot { get; }

        public string Title { get; private set; }

        public Location Location { get; set; }

        public RectangleSize Viewport { get; set; }

        public string ScreenshotUrl { get; set; }

        public string DomUrl { get; set; }
    }
}
