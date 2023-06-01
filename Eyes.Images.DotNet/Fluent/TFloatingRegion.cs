using Applitools.Commands;
using Applitools.Fluent;

namespace Applitools
{
    public abstract class TFloatingRegion
    {
        public Padding Offset { get; set; }
    }

    public class RectangleFloatingRegion : TFloatingRegion
    {
        public UniversalRegion Region { get; set; }
    }

    public class SelectorFloatingRegion : TFloatingRegion
    {
        public RegionSelector Region { get; set; }
    }

    public class ElementFloatingRegion : TFloatingRegion
    {
        public RegionElement Region { get; set; }
    }
}