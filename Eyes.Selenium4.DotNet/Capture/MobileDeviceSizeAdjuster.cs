using Applitools.Capture;
using Applitools.Utils;
using System;
using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium.Capture
{
    class MobileDeviceSizeAdjuster : ISizeAdjuster
    {
        private const string GetViewportMetaTagContentScript =
            "var meta = document.querySelector('head > meta[name=viewport]');" +
            "var viewport = (meta == null) ? '' : meta.getAttribute('content');" +
            "return viewport;";

        protected ViewportMetaTag viewportMetaTag_;

        public MobileDeviceSizeAdjuster(IEyesJsExecutor jsExecutor)
        {
            string viewportMetaTagContent = (string)jsExecutor.ExecuteScript(GetViewportMetaTagContentScript);
            viewportMetaTag_ = ViewportMetaTag.ParseViewportMetaTag(viewportMetaTagContent);
        }

        public Region AdjustRegion(Region inputRegion, Size deviceLogicalViewportSize)
        {
            if (viewportMetaTag_.FollowDeviceWidth)
            {
                return inputRegion;
            }
            float widthRatio = (float)inputRegion.Width / deviceLogicalViewportSize.Width;
            Region adjustedRegion = new Region(inputRegion.Left, inputRegion.Top, deviceLogicalViewportSize.Width, (int)Math.Round(inputRegion.Height / widthRatio));
            return adjustedRegion;
        }
    }
}
