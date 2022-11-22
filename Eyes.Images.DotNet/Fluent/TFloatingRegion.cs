using Applitools.Commands;
using Applitools.Fluent;

namespace Applitools
{
    public abstract class TFloatingRegion
    {
    }

    public class RectangleFloatingRegion : TFloatingRegion
    {
        public UniversalRegion Region { get; set; }
        public Padding Offset { get; set; }
    }

    public class RegionFloatingRegion : TFloatingRegion
    {
        public RegionSelector Region { get; set; }
        public Padding Offset { get; set; }
    }

    public class ElementFloatingRegion : TFloatingRegion
    {
        // ? Offset
        public string ElementId { get; set; }
    }
}