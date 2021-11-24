using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools.Selenium
{
    /// <summary>
    /// Encapsulates implementations for providing region visibility during checkRegion.
    /// </summary>
    public interface IRegionVisibilityStrategy
    {
        void MoveToRegion(IPositionProvider positionProvider, Location location);

        void ReturnToOriginalPosition(IPositionProvider positionProvider);
    }
}
