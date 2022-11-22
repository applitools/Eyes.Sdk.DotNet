using Applitools.Utils.Geometry;

namespace Applitools.Metadata
{
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
    public partial class ImageMatchSettings
    {
        public MatchLevel MatchLevel { get; set; }

        public Region[] Ignore { get; set; }

        public Region[] Strict { get; set; }

        public Region[] Content { get; set; }

        public Region[] Layout { get; set; }

        public FloatingMatchSettings[] Floating { get; set; }

        public AccessibilityRegionByRectangle[] Accessibility { get; set; }

        public AccessibilitySettings AccessibilitySettings { get; set; }

        public long SplitTopHeight { get; set; }

        public long SplitBottomHeight { get; set; }

        public bool IgnoreCaret { get; set; }

        public bool IgnoreDisplacements { get; set; }

        public bool UseDom { get; set; }

        public bool EnablePatterns { get; set; }

        public long Scale { get; set; }

        public long Remainder { get; set; }
    }
#pragma warning restore CS1591 // Missing XML comment for publicly visible type or member
}