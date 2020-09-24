using System.Collections.Generic;
using Applitools.Utils.Geometry;

namespace Applitools.Fluent
{
    public interface IGetRegions
    {
        IList<IMutableRegion> GetRegions(Common.IEyesBase eyesBase, IEyesScreenshot screenshot);
    }
}