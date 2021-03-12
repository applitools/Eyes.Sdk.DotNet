using Applitools.Capture;
using Applitools.Utils;
using Applitools.Utils.Images;
using OpenQA.Selenium;
using System;
using System.Drawing;

namespace Applitools.Appium.Capture
{
    internal class TakesScreenshotImageProvider : IImageProvider
    {
        private Logger logger_;
        private ITakesScreenshot tsInstance_;
        private IJavaScriptExecutor jsExecutor_;
        private Eyes eyes_;

        public TakesScreenshotImageProvider(Logger logger, ITakesScreenshot tsInstance, Eyes eyes)
        {
            logger_ = logger;
            tsInstance_ = tsInstance;
            jsExecutor_ = (IJavaScriptExecutor)tsInstance;
            eyes_ = eyes;
        }

        public Bitmap GetImage()
        {
            byte[] screenshotBytes;
            Bitmap result;
            if (eyes_.CachedViewport.IsEmpty)
            {
                try
                {
                    string screenshot64 = (string)jsExecutor_.ExecuteScript("mobile: viewportScreenshot");
                    screenshotBytes = Convert.FromBase64String(screenshot64);
                }
                catch
                {
                    screenshotBytes = tsInstance_.GetScreenshot().AsByteArray;
                }
                result = BasicImageUtils.CreateBitmap(screenshotBytes);
            }
            else
            {
                screenshotBytes = tsInstance_.GetScreenshot().AsByteArray;
                Bitmap screenshotBitmap = BasicImageUtils.CreateBitmap(screenshotBytes);
                eyes_.DebugScreenshotProvider.Save(screenshotBitmap, "DEVICE_ORIGINAL");
                result = BasicImageUtils.Crop(screenshotBitmap, eyes_.CachedViewport);
            }
            result = BasicImageUtils.ScaleImage(result, eyes_.ScaleRatio);
            return result;
        }
    }
}