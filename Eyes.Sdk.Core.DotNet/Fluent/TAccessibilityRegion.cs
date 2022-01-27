using Applitools.Fluent;

namespace Applitools
{
    public abstract class TAccessibilityRegion
    {
        public AccessibilityRegionType Type { get; set; }
    }

    public class RectangleAccessibilityRegion : TAccessibilityRegion
    {
        public RegionRectangle Region { get; set; }
    }

    public class SelectorAccessibilityRegion : TAccessibilityRegion
    {
        public RegionSelector Region { get; set; }
    }

    public class ElementAccessibilityRegion : TAccessibilityRegion
    {
        public RegionElement Region { get; set; }
    }
}