using System.Collections.Generic;

namespace Applitools
{
    public interface IGetAccessibilityRegion
    {
        IList<AccessibilityRegionByRectangle> GetRegions(Common.IEyesBase eyesBase, IEyesScreenshot screenshot);
    }
}