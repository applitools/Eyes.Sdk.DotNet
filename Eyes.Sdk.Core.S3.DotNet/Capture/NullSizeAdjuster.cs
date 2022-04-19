using System.Drawing;
using Applitools.Capture;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    public class NullSizeAdjuster : ISizeAdjuster
    {
        public static ISizeAdjuster Instance { get; } = new NullSizeAdjuster();

        public Region AdjustRegion(Region inputRegion, Size deviceLogicalViewportSize)
        {
            return inputRegion;
        }
    }
}