using Applitools.Commands;

namespace Applitools.Fluent
{
    public class TRegion
    {
    }

    public class RegionRectangle : TRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class RegionSelector : TRegion, IFrame
    {
        public string Type { get; set; }
        public string Selector { get; set; }
    }
    
    public class RegionElement : TRegion
    {
        public string ElementId { get; set; }
    }

    public interface IGetRegions<T> where T : TRegion
    {
        T ToRegion();
    }
}