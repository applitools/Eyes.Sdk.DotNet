using System;
using System.Drawing;
using Applitools.Selenium.Scrolling;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.Utils.Images;
using OpenQA.Selenium;

namespace Applitools.Selenium.Capture
{
    internal class InternetExplorerScreenshotImageProvider : TakeScreenshotImageProvider
    {
        private SeleniumEyes eyes_;
        private readonly IEyesJsExecutor jsExecutor_;

        public InternetExplorerScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
            : base(logger, tsInstance, userAgent)
        {
            eyes_ = eyes;
            jsExecutor_ = new SeleniumJavaScriptExecutor(eyes.GetDriver());
        }

        public override Bitmap GetImage()
        {
            logger_.Verbose("Getting current position...");
            Point loc;
            double scaleRatio = eyes_.DevicePixelRatio;

            FrameChain currentFrameChain = eyes_.GetDriver().GetFrameChain();
            IPositionProvider positionProvider = null;
            if (currentFrameChain.Count == 0)
            {
                IWebElement scrollRootElement = eyes_.GetCurrentFrameScrollRootElement();
                positionProvider = SeleniumPositionProviderFactory.GetInstance(eyes_).GetPositionProvider(logger_, StitchModes.Scroll,
                    jsExecutor_, scrollRootElement, userAgent_);
                loc = positionProvider.GetCurrentPosition();
            }
            else
            {
                loc = currentFrameChain.GetDefaultContentScrollPosition();
            }
            Point scaledLoc = new Point((int)Math.Round(loc.X * scaleRatio), (int)Math.Round(loc.Y * scaleRatio));

            Bitmap image = base.GetImage();
            EyesWebDriver driver = eyes_.GetDriver();
            RectangleSize originalViewportSize = EyesSeleniumUtils.GetViewportSize(logger_, driver);
            RectangleSize viewportSize = originalViewportSize.Scale(scaleRatio);

            if (image.Height > viewportSize.Height || image.Width > viewportSize.Width)
            {
                //Damn IE driver returns full page screenshot even when not asked to!
                logger_.Verbose("seems IE returned full page screenshot rather than only the viewport.");
                eyes_.DebugScreenshotProvider.Save(image, "IE");
                if (!eyes_.IsCutProviderExplicitlySet)
                {
                    Bitmap croppedImage = BasicImageUtils.Crop(image, new Rectangle(scaledLoc, viewportSize));
                    image.Dispose();
                    image = croppedImage;
                }
            }

            positionProvider?.SetPosition(loc);
            return image;
        }
    }
}
