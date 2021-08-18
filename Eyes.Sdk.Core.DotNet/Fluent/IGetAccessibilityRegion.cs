using System.Collections.Generic;

namespace Applitools
{
    public interface IGetAccessibilityRegion
    {
        IList<AccessibilityRegionByRectangle> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot);
    }
}