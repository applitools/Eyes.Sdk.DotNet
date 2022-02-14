using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Capture
{
    public interface ISizeAdjuster
    {
        Region AdjustRegion(Region fullarea, Size initialSizeScaled);
    }
}
