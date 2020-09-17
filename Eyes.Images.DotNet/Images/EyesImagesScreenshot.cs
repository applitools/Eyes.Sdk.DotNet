namespace Applitools.Images
{
    using Applitools.Utils.Images;
    using System.Drawing;
    using Utils.Geometry;
    using Region = Utils.Geometry.Region;

    class EyesImagesScreenshot : EyesScreenshot
    {
        public EyesImagesScreenshot(Bitmap image) : base(image)
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
            Region asIsSubScreenshotRegion = GetIntersectedRegion(region, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            if (asIsSubScreenshotRegion.IsSizeEmpty || (throwIfClipped && !asIsSubScreenshotRegion.Size.Equals(region.Size)))
            {
                throw new OutOfBoundsException($"Region [{region}] is out of screenshot bounds [{Image.Size}]");
            }

            Bitmap subScreenshotImage = BasicImageUtils.Crop(Image, asIsSubScreenshotRegion.ToRectangle());

            EyesImagesScreenshot result = new EyesImagesScreenshot(subScreenshotImage);
            return result;
        }
    }
}
