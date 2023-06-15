using System.Drawing;
using Applitools.Fluent;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public class SimpleRegionByRectangle : IGetRegions
    {
        private readonly Padding padding_;
        private readonly string regionId_;
        private readonly IMutableRegion region_;

        public SimpleRegionByRectangle(Rectangle region, Padding padding = null, string regionId = null)
        {
            padding_ = padding;
            regionId_ = regionId;
            region_ = new MutableRegion(region);
        }

        public SimpleRegionByRectangle(Point location, Size size) : this(new Rectangle(location, size)) { }
        
        public CodedRegionReference ToRegion()
        {
            return new CodedRegionReference
            {
                Region = new RegionRectangle
                {
                    X = region_.Left,
                    Y = region_.Top,
                    Width = region_.Width,
                    Height = region_.Height
                },
                Padding = padding_,
                RegionId = regionId_
            };
        }
    }
}