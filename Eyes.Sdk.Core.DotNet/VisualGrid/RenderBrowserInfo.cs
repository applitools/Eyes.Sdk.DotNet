using Applitools.Selenium;
using Applitools.Utils.Geometry;

namespace Applitools.VisualGrid
{
    public class RenderBrowserInfo
    {
        public RenderBrowserInfo(DesktopBrowserInfo desktopBrowserInfo, string baselineEnvName = null)
        {
            DesktopBrowserInfo = desktopBrowserInfo;
            BaselineEnvName = baselineEnvName;
            BrowserType = desktopBrowserInfo.BrowserType;
            Width = desktopBrowserInfo.Width;
            Height = desktopBrowserInfo.Height;
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


        public DesktopBrowserInfo DesktopBrowserInfo { get; }
        public EmulationBaseInfo EmulationInfo { get; }
        public IosDeviceInfo IosDeviceInfo { get; }

        public string BaselineEnvName { get; }
        public SizeMode Target { get; } = SizeMode.FullPage;
        public BrowserType BrowserType { get; }

        public int Width { get; }
        public int Height { get; }
        public RectangleSize ViewportSize => new RectangleSize(Width, Height);

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
                            return "windows";
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
