using Applitools.Utils;
using Applitools.Utils.Images;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Selenium.Capture
{
    internal class SafariScreenshotImageProvider : MobileScreenshotImageProvider
    {
        public SafariScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
            : base(eyes, logger, tsInstance, userAgent)
        {
        }

        public override Bitmap GetImage()
        {
            Bitmap image = base.GetImage();

            if (eyes_.CachedSessionDetails != null &&
                eyes_.CachedSessionDetails.TryGetValue("deviceOrientation", out object orientation))
            {
                if ("landscape".Equals(orientation as string, StringComparison.OrdinalIgnoreCase) && image.Width < image.Height)
                {
                    logger_.Verbose("rotating image...");
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
            }

            logger_.Verbose("Bitmap Size: {0}x{1}", image.Width, image.Height);
            eyes_.DebugScreenshotProvider.Save(image, "SAFARI");

            if (eyes_.IsCutProviderExplicitlySet)
            {
                return image;
            }

            double scaleRatio = eyes_.DevicePixelRatio;

            Size originalViewportSize = GetViewportSize();

            Size viewportSize = new Size(
                (int)Math.Ceiling(originalViewportSize.Width * scaleRatio),
                (int)Math.Ceiling(originalViewportSize.Height * scaleRatio));

            logger_.Verbose("logical viewport size: " + originalViewportSize);

            Bitmap croppedImage = null;
            if (userAgent_.IsiOS ||
                "ios".Equals(eyes_.PlatformName, StringComparison.OrdinalIgnoreCase) ||
                (eyes_.CachedSessionDetails.TryGetValue("PlatformName", out object platformName) &&
                    "ios".Equals(platformName as string, StringComparison.OrdinalIgnoreCase)))
            {
                croppedImage = IosCropHelper.CropIOSImage(image, originalViewportSize, logger_);
            }
            else if (!eyes_.ForceFullPageScreenshot)
            {
                Point loc;
                FrameChain currentFrameChain = eyes_.GetDriver().GetFrameChain();

                if (currentFrameChain.Count == 0)
                {
                    IWebElement scrollRootElement = eyes_.GetCurrentFrameScrollRootElement();
                    IPositionProvider positionProvider = eyes_.GetPositionProvider(
                        logger_, StitchModes.Scroll, jsExecutor_, scrollRootElement,
                        userAgent_);
                    loc = positionProvider.GetCurrentPosition();
                }
                else
                {
                    loc = currentFrameChain.GetDefaultContentScrollPosition();
                }

                loc = new Point((int)Math.Ceiling(loc.X * scaleRatio), (int)Math.Ceiling(loc.Y * scaleRatio));

                croppedImage = BasicImageUtils.Crop(image, new Rectangle(loc, viewportSize));
            }
            if (croppedImage != null && !ReferenceEquals(croppedImage, image))
            {
                image.Dispose();
                image = croppedImage;
            }
            eyes_.DebugScreenshotProvider.Save(image, "SAFARI_CROPPED");

            return image;
        }
    }
}