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
            TestResults = closeAllResult.Result;
            BrowserInfo = CreateRenderBrowserInfo(closeAllResult.Renderer);
            if (closeAllResult.Error != null)
            {
                Exception = new Exception(closeAllResult.Error.Message);
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

        private RenderBrowserInfo CreateRenderBrowserInfo(Renderer renderer)
        {
            if (renderer == null)
            {
                return null;
            }
            var chromeEmulationInfo = renderer.ChromeEmulationInfo;
            if (chromeEmulationInfo != null)
            {
                var name = chromeEmulationInfo.DeviceName.ToDeviceName();
                var orientation = chromeEmulationInfo.ScreenOrientation.ToScreenOrientation();
                return new RenderBrowserInfo(new ChromeEmulationInfo(name, orientation));
            }
            var android = renderer.AndroidDeviceInfo;
            if (android != null)
            {
                var name = android.DeviceName.ToAndroidDeviceName();
                var orientation = android.ScreenOrientation.ToScreenOrientation();
                return new RenderBrowserInfo(new AndroidDeviceInfo(name, orientation));
            }
            var ios = renderer.IosDeviceInfo;
            if (ios != null)
            {
                var name = ios.DeviceName.ToIosDeviceName();
                var orientation = ios.ScreenOrientation.ToScreenOrientation();
                return new RenderBrowserInfo(new IosDeviceInfo(name, orientation));
            }
            if (string.IsNullOrEmpty(renderer.Name) == false)
            {
                var browserType = renderer.Name.ToBrowserType();
                return new RenderBrowserInfo(new DesktopBrowserInfo(
                    renderer.Width.Value, 
                    renderer.Height.Value,
                    browserType));
            }

            return null;
        }
    }
}