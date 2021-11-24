using Applitools.Positioning;
using Applitools.Selenium.Common;
using Applitools.Utils;

namespace Applitools.Selenium.Positioning
{
    internal class RegionPositionCompensationFactory
    {
        internal static IRegionPositionCompensation GetRegionPositionCompensation(UserAgent userAgent, ISeleniumEyesCommon eyes, Logger logger)
        {
            if (userAgent != null)
            {
                if (userAgent.Browser.Equals(BrowserNames.Firefox))
                {
                    int.TryParse(userAgent.BrowserMajorVersion, out int firefoxMajorVersion);
                    if (firefoxMajorVersion >= 48)
                    {
                        return new FirefoxRegionPositionCompensation(eyes, logger);
                    }
                }
                else if (userAgent.Browser.Equals(BrowserNames.Safari) && userAgent.OS.Equals(OSNames.MacOSX))
                {
                    return new SafariRegionPositionCompensation();
                }
                else if (userAgent.Browser.Equals(BrowserNames.IE))
                {
                    return new InternetExplorerRegionPositionCompensation();
                }
            }
            
            return new NullRegionPositionCompensation();
        }
    }
}
