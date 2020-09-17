namespace Applitools.Selenium
{
    using Utils;
    using Utils.Geometry;
    using System.Drawing;

    /// <summary>
    /// An implementation of <see cref="IRegionVisibilityStrategy"/> which tries to move the region.
    /// </summary>
    public class MoveToRegionVisibilityStrategy : IRegionVisibilityStrategy
    {
        private static readonly int VISIBILITY_OFFSET = 100; // Pixels

        private Logger Logger_;
        private PositionMemento OriginalPosition_;

        public MoveToRegionVisibilityStrategy(Logger logger)
        {
            Logger_ = logger;
        }

        public void MoveToRegion(IPositionProvider positionProvider, Location location)
        {
            Logger_.Verbose("Getting current position state..");
            OriginalPosition_ = positionProvider.GetState();
            Logger_.Verbose("Done! Setting position...");

            // We set the location to "almost" the location we were asked. This is because sometimes, moving the browser
            // to the specific pixel where the element begins, causes the element to be slightly out of the viewport.
            int dstX = location.X - VISIBILITY_OFFSET;
            dstX = dstX < 0 ? 0 : dstX;
            int dstY = location.Y - VISIBILITY_OFFSET;
            dstY = dstY < 0 ? 0 : dstY;
            positionProvider.SetPosition(new Point(dstX, dstY));
            
            Logger_.Verbose("Done!");
        }

        public void ReturnToOriginalPosition(IPositionProvider positionProvider)
        {
            Logger_.Verbose("Returning to original position...");
            positionProvider.RestoreState(OriginalPosition_);
            Logger_.Verbose("Done!");
        }
    }
}
