namespace Applitools
{
    using System;
    using System.Drawing;
    using System.IO;
    using Utils.Geometry;
    using Utils.Images;
    using Region = Utils.Geometry.Region;

    class EyesLeanFTScreenshot : EyesScreenshot
    {
        public EyesLeanFTScreenshot(byte[] screenshotBytes) : base(BasicImageUtils.CreateBitmap(screenshotBytes)) { }

        public EyesLeanFTScreenshot(Bitmap image) : base(image)
        {
        }

        public override Point ConvertLocation(Point location, CoordinatesTypeEnum from, CoordinatesTypeEnum to)
        {
            return location;
        }

        public override Region GetIntersectedRegion(Region region, CoordinatesTypeEnum originalCoordinatesType, CoordinatesTypeEnum resultCoordinatesType)
        {
            return region;
        }

        public override Point GetLocationInScreenshot(Point location, CoordinatesTypeEnum coordinatesType)
        {
            return location;
        }

        public override EyesScreenshot GetSubScreenshot(Region region, bool throwIfClipped)
        {
            return this;
        }
    }
}
