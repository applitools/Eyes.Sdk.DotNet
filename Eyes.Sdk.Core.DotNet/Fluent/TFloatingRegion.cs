using Applitools.Fluent;

namespace Applitools
{
    public abstract class TFloatingRegion
    {
        public int MaxUpOffset { get; set; }
        public int MaxDownOffset { get; set; }
        public int MaxLeftOffset { get; set; }
        public int MaxRightOffset { get; set; }
    }

    public class RectangleFloatingRegion : TFloatingRegion
    {
        public RectangleRegion Region { get; set; }
    }

    public class SelectorFloatingRegion : TFloatingRegion
    {
        public SelectorRegion Region { get; set; }
    }

    public class ElementFloatingRegion : TFloatingRegion
    {
        public ElementRegion Region { get; set; }
    }
}