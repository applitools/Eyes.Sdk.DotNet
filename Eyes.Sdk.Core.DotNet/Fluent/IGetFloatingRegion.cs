using System.Collections.Generic;

namespace Applitools
{
    public interface IGetFloatingRegion
    {
        IList<FloatingMatchSettings> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot);
    }
}