using Applitools.Fluent;
using Applitools.Selenium.Commands;

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
        public string ElementId { get; set; }
    }
}