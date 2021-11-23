using System.Drawing;
using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools.Selenium.Capture
{
    internal class AndroidScreenshotImageProvider : MobileScreenshotImageProvider
    {
        public AndroidScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
            : base(eyes, logger, tsInstance, userAgent)
        {
        }

        public override Bitmap GetImage()
        {
            Bitmap image = base.GetImage();
            return AndroidCropHelper.CropAndroidImage(image, eyes_, logger_, GetViewportSize);
        }
    }
}
