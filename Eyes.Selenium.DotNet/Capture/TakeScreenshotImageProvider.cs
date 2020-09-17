using Applitools.Capture;
using Applitools.Utils;
using Applitools.Utils.Images;
using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium.Capture
{
    internal class TakeScreenshotImageProvider : IImageProvider
    {
        protected readonly Logger logger_;
        protected ITakesScreenshot tsInstance_;
        protected UserAgent userAgent_;

        public TakeScreenshotImageProvider(Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
        {
            logger_ = logger;
            tsInstance_ = tsInstance;
            userAgent_ = userAgent;
        }

        public virtual Bitmap GetImage()
        {
            return BasicImageUtils.CreateBitmap(tsInstance_.GetScreenshot().AsByteArray);
        }
    }
}