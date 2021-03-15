using Applitools.Appium;
using Applitools.Cropping;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.Utils.Images;
using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    class EyesAppiumScreenshot : EyesScreenshot
    {
        private readonly Logger logger_;
        private readonly Region workingArea_;
        private readonly Eyes eyes_;

        public EyesAppiumScreenshot(Logger logger, Bitmap image, Region workingArea, Eyes eyes) : base(image)
        {
            logger_ = logger;
            workingArea_ = workingArea;
            eyes_ = eyes;

            logger.Verbose(nameof(workingArea) + ": {0}", workingArea);
        }

        public override Point ConvertLocation(Point location, CoordinatesTypeEnum from, CoordinatesTypeEnum to)
        {
            if (from == to)
            {
                return location;
            }
            Point updatedLocation = new Point(location.X - workingArea_.Left, location.Y - workingArea_.Top);
            return updatedLocation;
        }

        public override Region GetIntersectedRegion(Region region, CoordinatesTypeEnum originalCoordinatesType, CoordinatesTypeEnum resultCoordinatesType)
        {
            return region;
        }

        public override Point GetLocationInScreenshot(Point location, CoordinatesTypeEnum coordinatesType)
        {
            return location;
        }

        public override EyesScreenshot GetSubScreenshot(Region subregion, bool throwIfClipped)
        {
            logger_.Verbose("GetSubScreenshot([{0}], {1})", subregion, throwIfClipped);

            ArgumentGuard.NotNull(subregion, nameof(subregion));
            if (eyes_.CutProvider != null && !(eyes_.CutProvider is NullCutProvider))
            {
                subregion = new Region(Point.Empty, workingArea_.Size);
            }
            Bitmap subScreenshotImage = BasicImageUtils.Crop(Image, subregion.ToRectangle());
            DisposeImage();

            subregion = subregion.Offset(workingArea_.Left, workingArea_.Top);
            EyesAppiumScreenshot result = new EyesAppiumScreenshot(logger_, subScreenshotImage, subregion, eyes_);

            logger_.Verbose("Done!");
            return result;
        }

        public override string ToString()
        {
            return base.ToString() + $" Region: {workingArea_}";
        }
    }
}