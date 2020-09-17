namespace Applitools.Selenium
{
    using Utils;
    using Utils.Geometry;

    /// <summary>
    /// An implementation of <see cref="IRegionVisibilityStrategy"/> which does nothing.
    /// </summary>
    public class NopRegionVisibilityStrategy : IRegionVisibilityStrategy
    {
        private readonly Logger Logger_;

        public NopRegionVisibilityStrategy(Logger logger)
        {
            this.Logger_ = logger;
        }

        public void MoveToRegion(IPositionProvider positionProvider, Location location)
        {
            Logger_.Verbose("Ignored (no op).");
        }

        public void ReturnToOriginalPosition(IPositionProvider positionProvider)
        {
            Logger_.Verbose("Ignored (no op).");
        }
    }
}
