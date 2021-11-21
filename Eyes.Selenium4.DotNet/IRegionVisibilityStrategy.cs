namespace Applitools.Selenium
{
    using Utils;
    using Utils.Geometry;

    /// <summary>
    /// Encapsulates implementations for providing region visibility during checkRegion.
    /// </summary>
    public interface IRegionVisibilityStrategy
    {
        void MoveToRegion(IPositionProvider positionProvider, Location location);

        void ReturnToOriginalPosition(IPositionProvider positionProvider);
    }
}
