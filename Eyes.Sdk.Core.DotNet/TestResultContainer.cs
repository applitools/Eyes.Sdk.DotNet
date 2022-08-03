using Applitools.VisualGrid;
using System;
using Applitools.Utils;

namespace Applitools
{
    public class TestResultContainer
    {
        public TestResults TestResults { get; }
        public RenderBrowserInfo BrowserInfo { get; }
        public Exception Exception { get; }

        public TestResultContainer(TestResults testResults, RenderBrowserInfo browserInfo = null, Exception exception = null)
        {
            TestResults = testResults;
            BrowserInfo = browserInfo;
            Exception = exception;
        }

        public TestResultContainer(CloseAllResult closeAllResult)
        {
            TestResults = closeAllResult.TestResults; 
            BrowserInfo = CreateRenderBrowserInfo(closeAllResult.BrowserInfo);
            if (closeAllResult.Exception != null)
            {
                Exception = new Exception(closeAllResult.Exception.Message);
            }
        }

        public override string ToString()
        {
            string browserInfoStr = BrowserInfo != null ? "\n browserInfo = " + BrowserInfo : string.Empty;
            return "TestResultContainer {" +
                    "\n testResults=" + TestResults +
                     browserInfoStr +
                    "\n exception = " + Exception +
                    '}';
        }

        private RenderBrowserInfo CreateRenderBrowserInfo(UniversalBrowserInfo universalBrowserInfo)
        {
            if (universalBrowserInfo == null)
            {
                return null;
            }

            var chromeEmulationInfo = universalBrowserInfo.ChromeEmulationInfo;
            if (chromeEmulationInfo != null)
            {
                var name = chromeEmulationInfo.DeviceName.ToDeviceName();
                var orientation = chromeEmulationInfo.ScreenOrientation.ToScreenOrientation();
                return new RenderBrowserInfo(new ChromeEmulationInfo(name, orientation));
            }
            var ios = universalBrowserInfo.IosDeviceInfo;
            if (ios != null)
            {
                var name = ios.DeviceName.ToIosDeviceName();
                var orientation = ios.ScreenOrientation.ToScreenOrientation();
                return new RenderBrowserInfo(new IosDeviceInfo(name, orientation));
            }
            if (string.IsNullOrEmpty(universalBrowserInfo.Name) == false)
            {
                var browserType = universalBrowserInfo.Name.ToBrowserType();
                return new RenderBrowserInfo(new DesktopBrowserInfo(
                    universalBrowserInfo.Width.Value,
                    universalBrowserInfo.Height.Value,
                    browserType));
            }

            return null;
        }
    }
}