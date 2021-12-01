using Applitools.Utils;
using Applitools.Utils.Images;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Capture
{
    class FirefoxScreenshotImageProvider : TakeScreenshotImageProvider
    {
        private SeleniumEyes eyes_;

        public FirefoxScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
             : base(logger, tsInstance, userAgent)
        {
            eyes_ = eyes;
        }

        public override Bitmap GetImage()
        {
            logger_.Verbose("Getting screenshot...");

            EyesWebDriver eyesWebDriver = eyes_.GetDriver();
            FrameChain frameChain = eyesWebDriver.GetFrameChain().Clone();

            logger_.Verbose("temporarilly switching to default content.");
            eyesWebDriver.SwitchTo().DefaultContent();

            Bitmap image = BasicImageUtils.CreateBitmap(tsInstance_.GetScreenshot().AsByteArray);
            eyes_.DebugScreenshotProvider.Save(image, "FIREFOX");

            logger_.Verbose("switching back to original frame.");
            ((EyesWebDriverTargetLocator)eyesWebDriver.SwitchTo()).Frames(frameChain);

            return image;
        }
    }
}
