using Applitools.Utils.Geometry;
using System.Drawing;

namespace Applitools.Selenium
{
    public interface IFrame
    {
        object ReferenceObj { get; }
        Size OuterSize { get; }
        Size InnerSize { get; }
        Point Location { get; }
        Rectangle Bounds { get; }
        RectangularMargins BorderWidths { get; }
        Point OriginalLocation { get; }
        Rectangle ScrollRootElementInnerBounds { get; set; }
    }
}