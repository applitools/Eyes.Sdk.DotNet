using System.Collections.Generic;

namespace Applitools
{
    public interface IGetAccessibilityRegion
    {
        IList<AccessibilityRegionByRectangle> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot);
    }
}