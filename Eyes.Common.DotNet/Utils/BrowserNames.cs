namespace Applitools.Utils
{
    public class BrowserNames
    {
        public const string Edge = "Edge";
        public const string EdgeChromium = "Edge Chromium";
        public const string IE = "IE";
        public const string Firefox = "Firefox";
        public const string Chrome = "Chrome";
        public const string Safari = "Safari";
        public const string Chromium = "Chromium";
        //public const string AndroidBrowser = "Android Browser";

        public static string GetBrowserName(BrowserType browserType)
        {
            switch (browserType)
            {
                case BrowserType.CHROME:
                case BrowserType.CHROME_ONE_VERSION_BACK:
                case BrowserType.CHROME_TWO_VERSIONS_BACK:
                    return Chrome;

                case BrowserType.FIREFOX:
                case BrowserType.FIREFOX_ONE_VERSION_BACK:
                case BrowserType.FIREFOX_TWO_VERSIONS_BACK: 
                    return Firefox;

                case BrowserType.SAFARI:
                case BrowserType.SAFARI_ONE_VERSION_BACK:
                case BrowserType.SAFARI_TWO_VERSIONS_BACK:
                    return Safari;

                case BrowserType.IE_10: return IE + " 10";
                case BrowserType.IE_11: return IE + " 11";

                case BrowserType.EDGE_LEGACY:
                case BrowserType.EDGE: return Edge;

                case BrowserType.EDGE_CHROMIUM:
                case BrowserType.EDGE_CHROMIUM_ONE_VERSION_BACK:
                case BrowserType.EDGE_CHROMIUM_TWO_VERSIONS_BACK: return EdgeChromium;
            }
            return null;
        }
    }
}