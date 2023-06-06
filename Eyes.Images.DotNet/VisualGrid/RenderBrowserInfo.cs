using System.ComponentModel;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid.Model;
using System.Drawing;
using Newtonsoft.Json;
using BrowserTypes = Applitools.BrowserType;

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
            BrowserType = BrowserTypes.SAFARI;
        }

        public RenderBrowserInfo(ChromeEmulationInfo chromeEmulationInfo, string baselineEnvName = null)
        {
            ChromeEmulationInfo = chromeEmulationInfo;
            BaselineEnvName = baselineEnvName;
        }

        public RenderBrowserInfo(AndroidDeviceInfo androidDeviceInfo, string baselineEnvName = null)
        {
            AndroidDeviceInfo = androidDeviceInfo;
            BaselineEnvName = baselineEnvName;
        }

        public RectangleSize GetDeviceSize()
        {
            return ViewportSize ??
                   ChromeEmulationInfo?.Size ??
                   IosDeviceInfo?.Size ??
                   Size.Empty;
        }

        public string GetDeviceName()
        {
            return ChromeEmulationInfo?.DeviceName.ToString() ??
                   IosDeviceInfo?.DeviceName.ToString() ??
                   AndroidDeviceInfo?.DeviceName.ToString() ??
                   string.Empty;
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
        public ChromeEmulationInfo ChromeEmulationInfo { get; }
        public IosDeviceInfo IosDeviceInfo { get; }
        public AndroidDeviceInfo AndroidDeviceInfo { get; }

        public string BaselineEnvName { get; }
        
        [DefaultValue(SizeMode.FullPage)]
        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public SizeMode Target { get; } = SizeMode.FullPage;
        
        [JsonProperty("name")]
        public BrowserType? BrowserType { get; }

        public int? Width => ViewportSize?.Width;
        public int? Height => ViewportSize?.Height;
        [JsonIgnore]
        public RectangleSize ViewportSize { get; set; }
        
        [JsonIgnore]
        public string Platform
        {
            get
            {
                if (IosDeviceInfo != null) return "ios";
                if (DesktopBrowserInfo != null)
                {
                    switch (DesktopBrowserInfo.BrowserType)
                    {
                        case BrowserTypes.IE_10:
                        case BrowserTypes.IE_11:
#pragma warning disable CS0618
                        case BrowserTypes.EDGE:
#pragma warning restore CS0618
                        case BrowserTypes.EDGE_LEGACY:
                        case BrowserTypes.EDGE_CHROMIUM:
                        case BrowserTypes.EDGE_CHROMIUM_ONE_VERSION_BACK:
                        case BrowserTypes.EDGE_CHROMIUM_TWO_VERSIONS_BACK:
                            return "windows";
                        case BrowserTypes.SAFARI:
                        case BrowserTypes.SAFARI_ONE_VERSION_BACK:
                        case BrowserTypes.SAFARI_TWO_VERSIONS_BACK:
                        case BrowserTypes.SAFARI_EARLY_ACCESS:
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
