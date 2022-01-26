using System;
using System.Drawing;
using Applitools.Utils;
using Applitools.Utils.Images;
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
            throw new EyesException();
        }
    }
}
