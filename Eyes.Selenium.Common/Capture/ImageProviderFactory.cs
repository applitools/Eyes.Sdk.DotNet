using Applitools.Capture;
using Applitools.Utils;
using System;
using System.Reflection;

namespace Applitools.Selenium.Capture
{
    internal static class ImageProviderFactory
    {
        public static IImageProvider GetImageProvider(UserAgent ua, IEyes eyes, Logger logger, object tsInstance)
        {
            Assembly asm = Assembly.GetCallingAssembly();
            Type factoryType = null;
            if (ua != null)
            {
                if (ua.Browser.Equals(BrowserNames.Firefox))
                {
                    int.TryParse(ua.BrowserMajorVersion, out int firefoxMajorVersion);
                    if (firefoxMajorVersion >= 48)
                    {
                        factoryType = asm.GetType("Applitools.Selenium.Capture.FirefoxScreenshotImageProvider");
                    }
                }
                else if (ua.Browser.Equals(BrowserNames.Safari))
                {
                    factoryType = asm.GetType("Applitools.Selenium.Capture.SafariScreenshotImageProvider");
                }
                else if (ua.Browser.Equals(BrowserNames.IE))
                {
                    factoryType = asm.GetType("Applitools.Selenium.Capture.InternetExplorerScreenshotImageProvider");
                }
                else if (ua.OS.Equals(OSNames.Android))
                {
                    factoryType = asm.GetType("Applitools.Selenium.Capture.AndroidScreenshotImageProvider");
                }
            }
            factoryType = asm.GetType("Applitools.Selenium.Capture.TakeScreenshotImageProvider");

            return (IImageProvider)Activator.CreateInstance(factoryType, eyes, logger, tsInstance, ua);
        }
    }
}
