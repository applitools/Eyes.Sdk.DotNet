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
            Bitmap image = base.GetImage();
            logger_.Verbose("Bitmap Size: {0}x{1}", image.Width, image.Height);

            eyes_.DebugScreenshotProvider.Save(image, "ANDROID");

            if (eyes_.IsCutProviderExplicitlySet)
            {
                return image;
            }

            Size originalViewportSize = GetViewportSize();

            logger_.Verbose("logical viewport size: " + originalViewportSize);

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            logger_.Verbose("physical device pixel size: {0}x{1}", imageWidth, imageHeight);
            float widthRatio = image.Width / (float)originalViewportSize.Width;
            float height = widthRatio * originalViewportSize.Height;
            Rectangle cropRect = new Rectangle(0, 0, imageWidth, (int)Math.Round(height));
            Bitmap croppedImage = BasicImageUtils.Crop(image, cropRect);
            image.Dispose();

            return croppedImage;
        }
    }
}
