using System.Drawing;
using System.Drawing.Imaging;
using Applitools.Utils.Geometry;

namespace Applitools
{
    internal class TestEyesScreenshot : EyesScreenshot
    {
        private static readonly Bitmap testBitmap = new Bitmap(100, 100, PixelFormat.Format32bppArgb);

        public TestEyesScreenshot() : base(testBitmap)
        {
        }

        public override Point ConvertLocation(Point location, CoordinatesTypeEnum from, CoordinatesTypeEnum to)
        {
            return location;
        }

        public override Utils.Geometry.Region GetIntersectedRegion(Utils.Geometry.Region region, CoordinatesTypeEnum originalCoordinatesType, CoordinatesTypeEnum resultCoordinatesType)
        {
            return region;
        }

        public override Point GetLocationInScreenshot(Point location, CoordinatesTypeEnum coordinatesType)
        {
            return location;
        }

        public override EyesScreenshot GetSubScreenshot(Utils.Geometry.Region region, bool throwIfClipped)
        {
            return new TestEyesScreenshot();
        }
    }
}