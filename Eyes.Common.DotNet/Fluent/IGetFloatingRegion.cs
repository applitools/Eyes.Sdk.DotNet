using System.Collections.Generic;

namespace Applitools
{
    public interface IGetFloatingRegion
    {
        IList<FloatingMatchSettings> GetRegions(Common.IEyesBase eyesBase, IEyesScreenshot screenshot);
    }
}