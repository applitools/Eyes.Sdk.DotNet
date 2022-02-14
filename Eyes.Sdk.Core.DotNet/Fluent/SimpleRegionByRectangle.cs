using System.Drawing;
using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public class SimpleRegionByRectangle : IGetRegions
    {
        private readonly IMutableRegion region;

        public SimpleRegionByRectangle(Rectangle region)
        {
            this.region = new MutableRegion(region);
        }

        public SimpleRegionByRectangle(Point location, Size size) : this(new Rectangle(location, size)) { }
        
        public TRegion ToRegion()
        {
            return new RegionRectangle
            {
                X = region.Left,
                Y = region.Top,
                Width = region.Width,
                Height = region.Height
            };
        }
    }
}