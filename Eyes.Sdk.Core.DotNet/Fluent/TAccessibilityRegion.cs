using Applitools.Fluent;

namespace Applitools
{
    public abstract class TAccessibilityRegion
    {
        public AccessibilityRegionType Type { get; set; }
    }

    public class RectangleAccessibilityRegion : TAccessibilityRegion
    {
        public RectangleRegion Region { get; set; }
    }

    public class SelectorAccessibilityRegion : TAccessibilityRegion
    {
        public SelectorRegion Region { get; set; }
    }

    public class ElementAccessibilityRegion : TAccessibilityRegion
    {
        public ElementRegion Region { get; set; }
    }
}