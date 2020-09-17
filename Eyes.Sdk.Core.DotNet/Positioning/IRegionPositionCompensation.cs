using Applitools.Utils.Geometry;

namespace Applitools.Positioning
{
    public interface IRegionPositionCompensation
    {
        Region CompensateRegionPosition(Region region, double pixelRatio);
    }
}
