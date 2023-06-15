using Applitools.Utils.Geometry;
using System.Drawing;

namespace Applitools
{
    public interface IEyesScreenshot
    {
        Point ConvertLocation(Point location, CoordinatesTypeEnum cONTEXT_RELATIVE, CoordinatesTypeEnum sCREENSHOT_AS_IS);
        Bitmap Image { get; }
    }
}