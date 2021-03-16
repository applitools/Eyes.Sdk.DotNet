using Applitools.Selenium;
using Applitools.Utils.Geometry;
using Applitools.VisualGrid.Model;
using System.Drawing;

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

        public RenderBrowserInfo(EmulationBaseInfo emulationInfo, string baselineEnvName = null)
        {
            EmulationInfo = emulationInfo;
            BaselineEnvName = baselineEnvName;
            BrowserType = BrowserType.CHROME;
        }

        public RectangleSize GetDeviceSize()
        {
            return ViewportSize ??
                   EmulationInfo?.Size ??
                   IosDeviceInfo.Size ??
                   Size.Empty;
        }

        public void SetEmulationDeviceSize(DeviceSize size)
        {
            if (size != null && EmulationInfo != null)
            {
                if (EmulationInfo.ScreenOrientation == ScreenOrientation.Portrait)
                {
                    EmulationInfo.Size = size.Portrait;
                }
                else
                {
                    EmulationInfo.Size = size.Landscape;
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

        public DesktopBrowserInfo DesktopBrowserInfo { get; }
        public EmulationBaseInfo EmulationInfo { get; }
        public IosDeviceInfo IosDeviceInfo { get; }

        public string BaselineEnvName { get; }
        public SizeMode Target { get; } = SizeMode.FullPage;
        public BrowserType BrowserType { get; }

        public int Width => ViewportSize?.Width ?? 0;
        public int Height => ViewportSize?.Height ?? 0;
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
                        case BrowserType.EDGE:
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
            return (DesktopBrowserInfo ?? IosDeviceInfo ?? (IRenderBrowserInfo)EmulationInfo).ToString();
        }
    }
}
