using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public abstract class TFloatingRegion
    {
    }

    public class RectangleFloatingRegion : TFloatingRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
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


    //public class RectangleFloatingRegion : TFloatingRegion // ?
    //{
    //    public RegionRectangle Region { get; set; }
    //}

    //public class SelectorFloatingRegion : TFloatingRegion
    //{
    //    public RegionSelector Region { get; set; }
    //}

    //public class ElementFloatingRegion : TFloatingRegion
    //{
    //    public RegionElement Region { get; set; }
    //}
}