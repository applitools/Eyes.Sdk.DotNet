using Applitools.Utils.Geometry;
using Applitools.VisualGrid.Model;
using System.Drawing;
using Newtonsoft.Json;

namespace Applitools.VisualGrid
{
    public class RenderBrowserInfo
    {
        public RenderBrowserInfo(DesktopBrowserInfo desktopBrowserInfo, string baselineEnvName = null)
        {
            DesktopBrowserInfo = desktopBrowserInfo;
            BaselineEnvName = baselineEnvName;
            BrowserType = desktopBrowserInfo.BrowserType;
            ViewportSize = new RectangleSize(desktopBrowserInfo.ViewportSize);
        }

        public RenderBrowserInfo(IosDeviceInfo deviceInfo, string baselineEnvName = null)
        {
            IosDeviceInfo = deviceInfo;
            BaselineEnvName = baselineEnvName;
            BrowserType = BrowserType.SAFARI;
        }

        public RenderBrowserInfo(EmulationBaseInfo chromeEmulationInfo, string baselineEnvName = null)
        {
            ChromeEmulationInfo = chromeEmulationInfo;
            BaselineEnvName = baselineEnvName;
            BrowserType = BrowserType.CHROME;
        }

        public RenderBrowserInfo(AndroidDeviceInfo androidDeviceInfo, string baselineEnvName = null)
        {
            AndroidDeviceInfo = androidDeviceInfo;
            BaselineEnvName = baselineEnvName;
            BrowserType = BrowserType.CHROME;
        }

        public RectangleSize GetDeviceSize()
        {
            return ViewportSize ??
                   ChromeEmulationInfo?.Size ??
                   IosDeviceInfo?.Size ??
                   Size.Empty;
        }

        public void SetEmulationDeviceSize(DeviceSize size)
        {
            if (size != null && ChromeEmulationInfo != null)
            {
                if (ChromeEmulationInfo.ScreenOrientation == ScreenOrientation.Portrait)
                {
                    ChromeEmulationInfo.Size = size.Portrait;
                }
                else
                {
                    ChromeEmulationInfo.Size = size.Landscape;
                }
            }
        }

        public void SetIosDeviceSize(DeviceSize size)
        {
            if (size != null && IosDeviceInfo != null)
            {
                if (IosDeviceInfo.ScreenOrientation == ScreenOrientation.Portrait)
                {
                    IosDeviceInfo.Size = size.Portrait;
                }
                else
                {
                    IosDeviceInfo.Size = size.Landscape;
                }
            }
        }

        [JsonIgnore]
        public DesktopBrowserInfo DesktopBrowserInfo { get; }
        public EmulationBaseInfo ChromeEmulationInfo { get; }
        public IosDeviceInfo IosDeviceInfo { get; }
        public AndroidDeviceInfo AndroidDeviceInfo { get; }

        public string BaselineEnvName { get; }
        public SizeMode Target { get; } = SizeMode.FullPage;
        [JsonProperty("name")]
        public BrowserType BrowserType { get; }

        public int? Width => ViewportSize?.Width;
        public int? Height => ViewportSize?.Height;
        [JsonIgnore]
        public RectangleSize ViewportSize { get; set; }
        public string Platform
        {
            get
            {
                if (IosDeviceInfo != null) return "ios";
                if (DesktopBrowserInfo != null)
                {
                    switch (DesktopBrowserInfo.BrowserType)
                    {
                        case BrowserType.IE_10:
                        case BrowserType.IE_11:
#pragma warning disable CS0618
                        case BrowserType.EDGE:
#pragma warning restore CS0618
                        case BrowserType.EDGE_LEGACY:
                        case BrowserType.EDGE_CHROMIUM:
                        case BrowserType.EDGE_CHROMIUM_ONE_VERSION_BACK:
                        case BrowserType.EDGE_CHROMIUM_TWO_VERSIONS_BACK:
                            return "windows";
                        case BrowserType.SAFARI:
                        case BrowserType.SAFARI_ONE_VERSION_BACK:
                        case BrowserType.SAFARI_TWO_VERSIONS_BACK:
                        case BrowserType.SAFARI_EARLY_ACCESS:
                            return "mac os x";
                    }
                }
                return "linux";
            }
        }

        public override string ToString()
        {
            return (DesktopBrowserInfo ?? IosDeviceInfo ?? (IRenderBrowserInfo)ChromeEmulationInfo).ToString();
        }
    }
}
