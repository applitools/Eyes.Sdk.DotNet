namespace Applitools.Fluent
{
    public class TRegion
    {

    }

    public class RectangleRegion : TRegion
    {
        public int X { get; set; }
        public int Y { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }

    public class SelectorRegion : TRegion
    {
        public string Type { get; set; }
        public string Selector { get; set; }
    }

    public class ElementRegion : TRegion
    {
        public string ElementId { get; set; }
    }

    public interface IGetRegions<T> where T : TRegion
    {
        T ToRegion();
    }


}