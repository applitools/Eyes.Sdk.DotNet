using Applitools.Utils.Geometry;
using Newtonsoft.Json;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace Applitools
{
    /// <summary>
    /// An application output.
    /// </summary>
    public sealed class AppOutput
    {
        public AppOutput(string title, Location location, Bitmap image, string domUrl = null, RectangleSize viewport = null)
        {
            Title = title;
            Location = location;
            ScreenshotUrl = null;
            DomUrl = domUrl;
            Viewport = viewport;
            using (MemoryStream stream = new MemoryStream())
            {
                image.Save(stream, ImageFormat.Png);
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

        public string Title { get; private set; }

        [JsonIgnore]
        public byte[] ScreenshotBytes { get; set; }

        public Location Location { get; set; }

        public RectangleSize Viewport { get; set; }

        public string ScreenshotUrl { get; set; }

        public string DomUrl { get; set; }
    }
}
