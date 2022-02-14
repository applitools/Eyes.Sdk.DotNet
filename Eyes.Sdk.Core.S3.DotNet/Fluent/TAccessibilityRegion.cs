using Applitools.Fluent;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace Applitools
{
    public abstract class TAccessibilityRegion
    {
        [JsonConverter(typeof(StringEnumConverter))]
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