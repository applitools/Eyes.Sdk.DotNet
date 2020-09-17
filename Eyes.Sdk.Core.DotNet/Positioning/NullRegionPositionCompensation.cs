using Applitools.Utils.Geometry;

namespace Applitools.Positioning
{
    public class NullRegionPositionCompensation : IRegionPositionCompensation
    {
        public Region CompensateRegionPosition(Region region, double pixelRatio)
        {
            return region;
        }
    }
}