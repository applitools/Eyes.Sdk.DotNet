using System.Text.RegularExpressions;

namespace Applitools.Utils
{
    public class ViewportMetaTag
    {
        //private static readonly Regex viewportParsingRegex_ = new Regex(
        //    @"(width\W*=\W*(?<width>[a-zA-Z0-9\.-]*))?,?\W*(initial-scale\W*=\W*(?<initialScale>[a-zA-Z0-9\.-]*))?",
        //    RegexOptions.Compiled);
        
        private static readonly Regex viewportParsingRegex_ = new Regex(
            @"(width\W*=\W*((?<width>[0-9]+)(px)?)|(?<deviceWidth>device-width))?,?\W*(initial-scale\W*=\W*(?<initialScale>[a-zA-Z0-9\.-]*))?",
            RegexOptions.Compiled);

        private float deviceWidth_;
        private float initialScale_;

        public bool FollowDeviceWidth { get; private set; }
        public float DeviceWidth { get => deviceWidth_; }
        public float InitialScale { get => initialScale_; }

        private ViewportMetaTag() { }

        public static ViewportMetaTag ParseViewportMetaTag(string viewportMetaTagContent)
        {
            MatchCollection viewportMatches = viewportParsingRegex_.Matches(viewportMetaTagContent);
            string widthStr = null, initialScaleStr = null;
            string isDeviceWidth = null;
            foreach (Match match in viewportMatches)
            {
                if (widthStr == null && match.Groups["width"].Success)
                {
                    widthStr = match.Groups["width"].Value;
                }
                if (initialScaleStr == null && match.Groups["initialScale"].Success)
                {
                    initialScaleStr = match.Groups["initialScale"].Value;
                }
                if (isDeviceWidth == null && match.Groups["deviceWidth"].Success)
                {
                    isDeviceWidth = match.Groups["deviceWidth"].Value;
                }
            }
            ViewportMetaTag viewportData = new ViewportMetaTag();
            viewportData.FollowDeviceWidth = isDeviceWidth != null;
            if (!viewportData.FollowDeviceWidth)
            {
                float.TryParse(widthStr, out viewportData.deviceWidth_);
            }
            float.TryParse(initialScaleStr, out viewportData.initialScale_);
            return viewportData;
        }

    }
}
