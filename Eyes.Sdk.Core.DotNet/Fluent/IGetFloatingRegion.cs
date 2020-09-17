using System.Collections.Generic;

namespace Applitools
{
    public interface IGetFloatingRegion
    {
        IList<FloatingMatchSettings> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot);
    }
}