using Applitools.Utils;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Capture
{
    internal class MobileScreenshotImageProvider : TakeScreenshotImageProvider
    {
        protected readonly SeleniumEyes eyes_;
        protected readonly IEyesJsExecutor jsExecutor_;
        private Size? cachedViewportSize_ = null;
        private string cachedUrl_;

        public MobileScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
            : base(logger, tsInstance, userAgent)
        {
            eyes_ = eyes;
            jsExecutor_ = new SeleniumJavaScriptExecutor(eyes.GetDriver());
        }

        protected Size GetViewportSize()
        {
            EyesWebDriver driver = eyes_.GetDriver();
            if (cachedViewportSize_ == null || cachedUrl_ != driver.Url)
            {
                cachedUrl_ = driver.Url;
                cachedViewportSize_ = EyesSeleniumUtils.GetViewportSize(logger_, driver);
            }
            Size originalViewportSize = cachedViewportSize_.Value;
            return originalViewportSize;
        }

    }
}