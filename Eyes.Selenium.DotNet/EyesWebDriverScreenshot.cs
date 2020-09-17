using Applitools.Selenium.Scrolling;
using OpenQA.Selenium;
using System.Drawing;
using Applitools.Utils;
using Applitools.Utils.Geometry;
using Applitools.Utils.Images;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Selenium
{
    public class EyesWebDriverScreenshot : EyesScreenshot
    {
        public enum ScreenshotTypeEnum
        {
            VIEWPORT, ENTIRE_FRAME
        }

        private Logger logger_;
        private readonly EyesWebDriver driver_;
        private Point currentFrameScrollPosition_;

        /// <summary>
        /// The part of the frame window which is visible in the screenshot 
        /// </summary>
        private readonly Region frameWindow_;

        /// <summary>
        /// The top/left coordinates of the frame window(!) relative to the top/left
        /// of the screenshot. Used for calculations, so can also be outside(!)
        /// the screenshot.
        /// </summary>
        private Point frameLocationInScreenshot_;
        private ScreenshotTypeEnum screenshotType_;
        private readonly FrameChain frameChain_;

        public EyesWebDriverScreenshot(Logger logger, EyesWebDriver driver, Bitmap image)
            : this(logger, driver, image, default(ScreenshotTypeEnum), null)
        {
        }

        public EyesWebDriverScreenshot(Logger logger, EyesWebDriver driver, Bitmap image, ScreenshotTypeEnum? screenshotType, Point? frameLocationInScreenshot)
            : base(image)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(driver, nameof(driver));
            logger_ = logger;
            driver_ = driver;

            logger_.Verbose("enter");

            UpdateScreenshotType_(screenshotType, image);

            IPositionProvider positionProvider;
            if (frameLocationInScreenshot == null || frameLocationInScreenshot.Value.IsEmpty)
            {
                IWebElement frameScrollRoot = driver.Eyes.GetCurrentFrameScrollRootElement();
                positionProvider = SeleniumPositionProviderFactory.GetPositionProvider(logger, driver.Eyes.StitchMode, driver, frameScrollRoot);
                logger_.Debug("position provider: using the current frame scroll root element's position provider: {0}", positionProvider);
            }
            else if (driver.Eyes.CurrentFramePositionProvider != null)
            {
                positionProvider = driver.Eyes.CurrentFramePositionProvider;
                logger_.Debug("position provider: using CurrentFramePositionProvider: {0}", positionProvider);
            }
            else
            {
                positionProvider = driver.Eyes.PositionProvider;
                logger_.Debug("position provider: using PositionProvider: {0}", positionProvider);
            }

            //IPositionProvider positionProvider = driver.Eyes.CurrentFramePositionProvider ?? driver.Eyes.PositionProvider;

            frameChain_ = driver_.GetFrameChain();
            logger_.Verbose("got frame chain. getting frame size...");
            Size frameSize = GetFrameSize_(positionProvider);
            UpdateCurrentScrollPosition_(positionProvider);
            UpdateFrameLocationInScreenshot_(frameLocationInScreenshot);
            Size frameContentSize = GetFrameContentSize();

            logger.Verbose("Calculating frame window...");
            frameWindow_ = new Region(frameLocationInScreenshot_, frameContentSize);
            Region imageSizeAsRegion = new Region(0, 0, image.Width, image.Height);
            logger.Verbose("frameWindow_: {0} ; imageSizeAsRegion: {1}", frameWindow_, imageSizeAsRegion);
            frameWindow_.Intersect(imageSizeAsRegion);
            logger.Verbose("updated frameWindow_: {0}", frameWindow_);

            if (frameWindow_.Width <= 0 || frameWindow_.Height <= 0)
            {
                throw new EyesException("Got empty frame window for screenshot!");
            }

            logger.Verbose("Done!");
        }

        private Size GetFrameContentSize()
        {
            IWebElement frameScrollRootElement = EyesSeleniumUtils.GetDefaultRootElement(driver_);
            Size sreSize = EyesRemoteWebElement.GetClientSize(frameScrollRootElement, driver_, logger_);
            return sreSize;
        }

        /// <summary>
        /// Create a frame(!) window screenshot
        /// </summary>
        /// <param name="logger"></param>
        /// <param name="driver"></param>
        /// <param name="image"></param>
        /// <param name="entireFrameSize"></param>
        public EyesWebDriverScreenshot(Logger logger, EyesWebDriver driver, Bitmap image, Size entireFrameSize, Point frameLocationInScreenshot)
            : base(image)
        {
            ArgumentGuard.NotNull(logger, nameof(logger));
            ArgumentGuard.NotNull(driver, nameof(driver));
            ArgumentGuard.NotNull(entireFrameSize, nameof(entireFrameSize));
            logger_ = logger;
            driver_ = driver;
            frameChain_ = driver_.GetFrameChain();
            // The frame comprises the entire screenshot.
            screenshotType_ = ScreenshotTypeEnum.ENTIRE_FRAME;
            currentFrameScrollPosition_ = new Point(0, 0);
            frameLocationInScreenshot_ = frameLocationInScreenshot;
            frameWindow_ = new Region(new Point(0, 0), entireFrameSize);
        }

        public Region FrameWindow { get { return frameWindow_; } }

        private void UpdateScreenshotType_(ScreenshotTypeEnum? screenshotType, Bitmap image)
        {
            if (screenshotType.HasValue)
            {
                logger_.Verbose("given {0} already have the value '{1}'. using it.", nameof(screenshotType), screenshotType.Value);
                screenshotType_ = screenshotType.Value;
                return;
            }

            Size viewportSize = driver_.GetDefaultContentViewportSize();
            if (image.Width <= viewportSize.Width && image.Height <= viewportSize.Height)
            {
                screenshotType_ = ScreenshotTypeEnum.VIEWPORT;
            }
            else
            {
                screenshotType_ = ScreenshotTypeEnum.ENTIRE_FRAME;
            }
            logger_.Verbose("updated screenshot type to {0}", screenshotType_);
        }

        private Size GetFrameSize_(IPositionProvider positionProvider)
        {
            Size frameSize;
            // If we're inside a frame, then the frame size is given by the frame
            // chain. Otherwise, it's the size of the entire page.
            if (frameChain_.Count != 0)
            {
                frameSize = frameChain_.GetCurrentFrameInnerSize();
            }
            else
            {
                // get entire page size might throw an exception for applications
                // which don't support Javascript (e.g., Appium). In that case
                // we'll use the viewport size as the frame's size.
                try
                {
                    logger_.Verbose("no framechain. positionProvider: {0}", positionProvider);
                    frameSize = positionProvider.GetEntireSize();
                    logger_.Verbose("frameSize: {0}", frameSize);
                }
                catch (EyesDriverOperationException)
                {
                    frameSize = driver_.GetDefaultContentViewportSize();
                }
            }

            return frameSize;
        }

        private void UpdateFrameLocationInScreenshot_(Point? location)
        {
            // This is used for frame related calculations.
            if (location == null)
            {
                if (frameChain_.Count > 0)
                {
                    frameLocationInScreenshot_ = CalcFrameLocationInScreenshot_(frameChain_, screenshotType_);
                }
                else
                {
                    frameLocationInScreenshot_ = new Point(0, 0);
                }
            }
            else
            {
                frameLocationInScreenshot_ = location.Value;
            }
        }

        private void UpdateCurrentScrollPosition_(IPositionProvider positionProvider)
        {
            // Getting the scroll position. For native Appium apps we can't get the
            // scroll position, so we use (0,0)
            try
            {
                currentFrameScrollPosition_ = positionProvider.GetCurrentPosition();
            }
            catch (EyesDriverOperationException)
            {
                currentFrameScrollPosition_ = new Point(0, 0);
            }
        }

        private Point CalcFrameLocationInScreenshot_(FrameChain frameChain, ScreenshotTypeEnum screenshotType)
        {
            EyesWebDriverTargetLocator switchTo = (EyesWebDriverTargetLocator)driver_.SwitchTo();
            FrameChain currentFC = frameChain.Clone();
            switchTo.DefaultContent();
            Point locationInScreenshot = Point.Empty;
            foreach (Frame frame in currentFC)
            {
                Rectangle rect = ((EyesRemoteWebElement)frame.Reference).GetClientBounds();
                SizeAndBorders sizeAndBorders = ((EyesRemoteWebElement)frame.Reference).SizeAndBorders;
                RectangularMargins borders = sizeAndBorders.Borders;
                rect.Offset(borders.Left, borders.Top);
                locationInScreenshot.Offset(rect.Location);
                switchTo.Frame(frame.Reference);
            }

            return locationInScreenshot;

        }

        public override EyesScreenshot GetSubScreenshot(Region region, bool throwIfClipped)
        {
            logger_.Verbose(nameof(GetSubScreenshot) + "([{0}], {1})", region, throwIfClipped);

            ArgumentGuard.NotNull(region, nameof(region));

            // We calculate intersection based on as-is coordinates.
            Region asIsSubScreenshotRegion = GetIntersectedRegion(region, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            if (asIsSubScreenshotRegion.IsSizeEmpty || (throwIfClipped && !asIsSubScreenshotRegion.Size.Equals(region.Size)))
            {
                throw new OutOfBoundsException($"Region [{region}] is out of screenshot bounds [{frameWindow_}]");
            }

            Bitmap subScreenshotImage = BasicImageUtils.Crop(Image, asIsSubScreenshotRegion.ToRectangle());

            // The frame location in the sub screenshot is the negative of the
            // context-as-is location of the region.
            Point contextAsIsRegionLocation =
                    ConvertLocation(asIsSubScreenshotRegion.Location,
                            CoordinatesTypeEnum.SCREENSHOT_AS_IS,
                            CoordinatesTypeEnum.CONTEXT_AS_IS);

            Point frameLocationInSubScreenshot = new Point(-contextAsIsRegionLocation.X, -contextAsIsRegionLocation.Y);

            EyesWebDriverScreenshot result = new EyesWebDriverScreenshot(logger_,
                    driver_, subScreenshotImage, subScreenshotImage.Size, Point.Empty);

            result.UpdateFrameLocationInScreenshot_(new Point(-region.Location.X, -region.Location.Y));

            result.DomUrl = DomUrl;

            logger_.Verbose("Done!");
            return result;
        }

        public override Point GetLocationInScreenshot(Point location, CoordinatesTypeEnum coordinatesType)
        {
            location = ConvertLocation(location, coordinatesType,
                CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            // Making sure it's within the screenshot bounds
            if (!frameWindow_.Contains(location))
            {
                throw new OutOfBoundsException($"Location {location} ('{coordinatesType}') is not visible in screenshot!");
            }
            return location;
        }

        public override Region GetIntersectedRegion(Region region, CoordinatesTypeEnum originalCoordinatesType, CoordinatesTypeEnum resultCoordinatesType)
        {
            if (region.IsSizeEmpty)
            {
                return region;
            }

            Region intersectedRegion = ConvertRegionLocation(region, originalCoordinatesType, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            switch (originalCoordinatesType)
            {
                // If the request was context based, we intersect with the frame window.
                case CoordinatesTypeEnum.CONTEXT_AS_IS:
                case CoordinatesTypeEnum.CONTEXT_RELATIVE:
                    intersectedRegion.Intersect(frameWindow_);
                    break;

                // If the request is screenshot based, we intersect with the image
                case CoordinatesTypeEnum.SCREENSHOT_AS_IS:
                    intersectedRegion.Intersect(new Region(0, 0, Image.Width, Image.Height));
                    break;

                default:
                    throw new CoordinatesTypeConversionException(string.Format("Unknown coordinates type: '{0}'", originalCoordinatesType));

            }

            // If the intersection is empty we don't want to convert the coordinates.
            if (intersectedRegion.IsSizeEmpty)
            {
                return intersectedRegion;
            }

            // Converting the result to the required coordinates type.
            intersectedRegion = ConvertRegionLocation(intersectedRegion, CoordinatesTypeEnum.SCREENSHOT_AS_IS, resultCoordinatesType);

            return intersectedRegion;
        }

        public override Point ConvertLocation(Point location, CoordinatesTypeEnum from, CoordinatesTypeEnum to)
        {
            ArgumentGuard.NotNull(location, nameof(location));
            ArgumentGuard.NotNull(from, nameof(from));
            ArgumentGuard.NotNull(to, nameof(to));

            if (from == to)
            {
                return location;
            }

            Point result = location;

            // If we're not inside a frame, and the screenshot is the entire
            // page, then the context as-is/relative are the same (notice
            // screenshot as-is might be different, e.g.,
            // if it is actually a sub-screenshot of a region).
            if (frameChain_.Count == 0 && screenshotType_ == ScreenshotTypeEnum.ENTIRE_FRAME)
            {
                if ((from == CoordinatesTypeEnum.CONTEXT_RELATIVE || from == CoordinatesTypeEnum.CONTEXT_AS_IS) && to == CoordinatesTypeEnum.SCREENSHOT_AS_IS)
                {
                    // If this is not a sub-screenshot, this will have no effect.
                    result.Offset(frameLocationInScreenshot_);

                }
                else if (from == CoordinatesTypeEnum.SCREENSHOT_AS_IS &&
                      (to == CoordinatesTypeEnum.CONTEXT_RELATIVE || to == CoordinatesTypeEnum.CONTEXT_AS_IS))
                {
                    result.Offset(-frameLocationInScreenshot_.X, -frameLocationInScreenshot_.Y);
                }
                return result;
            }

            switch (from)
            {
                case CoordinatesTypeEnum.CONTEXT_AS_IS:
                    switch (to)
                    {
                        case CoordinatesTypeEnum.CONTEXT_RELATIVE:
                            result.Offset(currentFrameScrollPosition_);
                            break;

                        case CoordinatesTypeEnum.SCREENSHOT_AS_IS:
                            result.Offset(frameLocationInScreenshot_);
                            break;

                        default:
                            throw new CoordinatesTypeConversionException(from, to);
                    }
                    break;

                case CoordinatesTypeEnum.CONTEXT_RELATIVE:
                    switch (to)
                    {
                        case CoordinatesTypeEnum.SCREENSHOT_AS_IS:
                            // First, convert context-relative to context-as-is.
                            result.Offset(-currentFrameScrollPosition_.X, -currentFrameScrollPosition_.Y);
                            // Now convert context-as-is to screenshot-as-is.
                            result.Offset(frameLocationInScreenshot_);
                            break;

                        case CoordinatesTypeEnum.CONTEXT_AS_IS:
                            result.Offset(-currentFrameScrollPosition_.X, -currentFrameScrollPosition_.Y);
                            break;

                        default:
                            throw new CoordinatesTypeConversionException(from, to);
                    }
                    break;

                case CoordinatesTypeEnum.SCREENSHOT_AS_IS:
                    switch (to)
                    {
                        case CoordinatesTypeEnum.CONTEXT_RELATIVE:
                            // First convert to context-as-is.
                            result.Offset(-frameLocationInScreenshot_.X, -frameLocationInScreenshot_.Y);
                            // Now convert to context-relative.
                            result.Offset(currentFrameScrollPosition_);
                            break;

                        case CoordinatesTypeEnum.CONTEXT_AS_IS:
                            result.Offset(-frameLocationInScreenshot_.X, -frameLocationInScreenshot_.Y);
                            break;

                        default:
                            throw new CoordinatesTypeConversionException(from, to);
                    }
                    break;

                default:
                    throw new CoordinatesTypeConversionException(from, to);
            }

            return result;
        }

    }
}
