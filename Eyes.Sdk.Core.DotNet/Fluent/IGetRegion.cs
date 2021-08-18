using System.Collections.Generic;
using Applitools.Utils.Geometry;

namespace Applitools.Fluent
{
    public interface IGetRegions
    {
        IList<IMutableRegion> GetRegions(IEyesBase eyesBase, IEyesScreenshot screenshot);
    }
}