using System;
using Applitools.Capture;
using Applitools.Utils;
using OpenQA.Selenium;

namespace Applitools.Selenium.Capture
{
    static class ImageProviderFactory
    {
        public static IImageProvider GetImageProvider(UserAgent ua, SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance)
        {
            if (ua != null)
            {
                if (ua.Browser.Equals(BrowserNames.Firefox))
                {
                    int.TryParse(ua.BrowserMajorVersion, out int firefoxMajorVersion);
                    if (firefoxMajorVersion >= 48)
                    {
                        return new FirefoxScreenshotImageProvider(eyes, logger, tsInstance, ua);
                    }
                }
                else if (ua.Browser.Equals(BrowserNames.Safari))
                {
                    return new SafariScreenshotImageProvider(eyes, logger, tsInstance, ua);
                }
                else if (ua.Browser.Equals(BrowserNames.IE))
                {
                    return new InternetExplorerScreenshotImageProvider(eyes, logger, tsInstance, ua);
                }
                else if (ua.OS.Equals(OSNames.Android))
                {
                    return new AndroidScreenshotImageProvider(eyes, logger, tsInstance, ua);
                }
            }
            return new TakeScreenshotImageProvider(logger, tsInstance, ua);
        }
    }
}
