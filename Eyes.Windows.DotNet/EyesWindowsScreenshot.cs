namespace Applitools
{
    using System.Drawing;
    using Utils.Geometry;
    using Region = Utils.Geometry.Region;

    class EyesWindowsScreenshot : EyesScreenshot
    {
        public EyesWindowsScreenshot(Bitmap image) : base(image)
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
