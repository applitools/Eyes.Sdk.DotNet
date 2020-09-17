namespace Applitools.Fluent
{
    using System.Collections.Generic;
    using Utils.Geometry;

    public interface IGetRegions
    {
        IList<IMutableRegion> GetRegions(EyesBase eyesBase, EyesScreenshot screenshot);
    }
}