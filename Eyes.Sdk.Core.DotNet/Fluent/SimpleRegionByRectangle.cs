using System.Drawing;
using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public class SimpleRegionByRectangle : IGetRegions
    {
        private readonly Padding _padding;
        private readonly string _regionId;
        private readonly IMutableRegion _region;

        public SimpleRegionByRectangle(Rectangle region, Padding padding = null, string regionId = null)
        {
            _padding = padding;
            _regionId = regionId;
            _region = new MutableRegion(region);
        }

        public SimpleRegionByRectangle(Point location, Size size) : this(new Rectangle(location, size)) { }
        
        public CodedRegionReference ToRegion()
        {
            return new CodedRegionReference
            {
                Region = new RegionRectangle
                {
                    X = _region.Left,
                    Y = _region.Top,
                    Width = _region.Width,
                    Height = _region.Height
                },
                Padding = _padding,
                RegionId = _regionId
            };
        }
    }
}