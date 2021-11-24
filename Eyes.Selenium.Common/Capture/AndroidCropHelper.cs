using Applitools.Utils.Images;
using System;
using System.Drawing;

namespace Applitools.Selenium.Capture
{
    internal class AndroidCropHelper
    {
        internal static Bitmap CropAndroidImage(Bitmap image, EyesBase eyes, Logger logger, Func<Size> getViewportSize)
        {
            logger.Verbose("Bitmap Size: {0}x{1}", image.Width, image.Height);

            eyes.DebugScreenshotProvider.Save(image, "ANDROID");

            if (eyes.IsCutProviderExplicitlySet)
            {
                return image;
            }

            Size originalViewportSize = getViewportSize();

            logger.Verbose("logical viewport size: " + originalViewportSize);

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            logger.Verbose("physical device pixel size: {0}x{1}", imageWidth, imageHeight);
            float widthRatio = image.Width / (float)originalViewportSize.Width;
            float height = widthRatio * originalViewportSize.Height;
            Rectangle cropRect = new Rectangle(0, 0, imageWidth, (int)Math.Round(height));
            Bitmap croppedImage = BasicImageUtils.Crop(image, cropRect);
            image.Dispose();

            return croppedImage;
        }
    }
}
