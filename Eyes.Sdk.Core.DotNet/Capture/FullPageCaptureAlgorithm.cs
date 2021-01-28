using Applitools.Capture;
using Applitools.Cropping;
using Applitools.Positioning;
using Applitools.Utils;
using Applitools.Utils.Cropping;
using Applitools.Utils.Geometry;
using Applitools.Utils.Images;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Threading;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools
{
    public class FullPageCaptureAlgorithm
    {
        private const int MinScreenshotPartSize_ = 10;

        private readonly Logger logger_;
        private readonly int waitBeforeScreenshots_;
        private readonly IDebugScreenshotProvider debugScreenshotsProvider_;
        private readonly Func<Bitmap, EyesScreenshot> getEyesScreenshot_;
        private readonly ScaleProviderFactory scaleProviderFactory_;
        private readonly ICutProvider cutProvider_;
        private readonly int stitchOverlap_;
        private readonly IImageProvider imageProvider_;
        private readonly ISizeAdjuster sizeAdjuster_;
        private readonly IRegionPositionCompensation regionPositionCompensation_;
        private readonly int maxHeight_;
        private readonly int maxArea_;

        private void SaveDebugScreenshotPart_(Bitmap image, Rectangle region, string name)
        {
            if (debugScreenshotsProvider_ is NullDebugScreenshotProvider) return;

            string suffix = $"part-{name}-{region.Left}_{region.Top}_{region.Width}x{region.Height}";
            debugScreenshotsProvider_.Save(image, suffix);
        }

        /// <summary>
        /// Create a new instance of <see cref="FullPageCaptureAlgorithm"/>.
        /// </summary>
        /// <param name="logger">The logger to use.</param>
        /// <param name="regionPositionCompensation">A class used to compensate for region offsets in various browsers.</param>
        /// <param name="waitBeforeScreenshots">The time to wait before a call for capturing a screenshot. Used mainly for allowing the page to stabilize after a position was set.</param>
        /// <param name="debugScreenshotsProvider">An object responsible for storing the intermediate images created in the process, for debugging purposes.</param>
        /// <param name="getEyesScreenshot">The function used to create an <see cref="EyesScreenshot"/> object from the actual screenshot image.</param>
        /// <param name="scaleProviderFactory">The scale provider factory.</param>
        /// <param name="cutProvider">The cut provider used for custom cropping.</param>
        /// <param name="stitchingOverlap">The amount of pixels to overlap when stitching.</param>
        /// <param name="imageProvider">The screenshot image provider.</param>
        /// <param name="maxHeight">The maximum image height acceptable by the server.</param>
        /// <param name="maxArea">The maximum image area (height x width) acceptable by the server.</param>
        /// <param name="sizeAdjuster">A size adjuster for the image. Needed for mobile devices that render desktop web pages.</param>
        public FullPageCaptureAlgorithm(Logger logger, IRegionPositionCompensation regionPositionCompensation,
                                  int waitBeforeScreenshots, IDebugScreenshotProvider debugScreenshotsProvider,
                                  Func<Bitmap, EyesScreenshot> getEyesScreenshot,
                                  ScaleProviderFactory scaleProviderFactory, ICutProvider cutProvider,
                                  int stitchingOverlap, IImageProvider imageProvider, int maxHeight, int maxArea,
                                  ISizeAdjuster sizeAdjuster = null)
        {

            ArgumentGuard.NotNull(logger, nameof(logger));

            logger_ = logger;
            waitBeforeScreenshots_ = waitBeforeScreenshots;
            debugScreenshotsProvider_ = debugScreenshotsProvider;
            getEyesScreenshot_ = getEyesScreenshot;
            scaleProviderFactory_ = scaleProviderFactory;
            cutProvider_ = cutProvider;
            stitchOverlap_ = stitchingOverlap;
            imageProvider_ = imageProvider;
            maxHeight_ = maxHeight;
            maxArea_ = maxArea;
            sizeAdjuster_ = sizeAdjuster ?? NullSizeAdjuster.Instance;
            regionPositionCompensation_ = regionPositionCompensation ?? new NullRegionPositionCompensation();
        }

        public Bitmap GetStitchedRegion(Region region, Region fullarea, IPositionProvider positionProvider,
            IPositionProvider originProvider)
        {
            return GetStitchedRegion(region, fullarea, positionProvider, originProvider, Size.Empty);
        }

        /// <summary>
        /// Encapsulates an algorithm for creating full-page images of a page.
        /// </summary>
        /// <param name="positionProvider">The position provider used for moving to the actual stitch points.</param>
        /// <param name="region">The region to stitch. If <see cref="Region.Empty"/>, the entire image will be stitched.</param>
        /// <param name="fullarea">The wanted area of the resulting image. If unknown, pass in <c>null</c> or <see cref="Region.Empty"/>.</param>
        /// <param name="originProvider">A position provider used for saving the state before 
        /// starting the stitching, as well as moving to (0,0). The reason it is separated from 
        /// the <c>stitchProvider</c>is that the stitchProvider might have side-effects 
        /// (e.g., changing the CSS transform of the page can cause a layout change at the 
        /// top of the page), which we can avoid for the first screenshot (since it might be a 
        /// full page screenshot anyway).</param>
        /// <param name="stitchOffset"></param>
        /// <returns>The screenshot as Bitmap.</returns>
        public Bitmap GetStitchedRegion(Region region, Region fullarea, IPositionProvider positionProvider,
        IPositionProvider originProvider, Size stitchOffset)
        {
            ArgumentGuard.NotNull(region, nameof(region));
            ArgumentGuard.NotNull(positionProvider, nameof(positionProvider));

            logger_.Verbose("region: {0} ; fullarea: {1} ; positionProvider: {2}",
                region, fullarea, positionProvider.GetType().Name);

            Point originalStitchedState = positionProvider.GetCurrentPosition();
            logger_.Verbose("region size: {0}, originalStitchedState: {1}", region, originalStitchedState);

            PositionMemento originProviderState = originProvider.GetState();
            logger_.Verbose("originProviderState: {0}", originProviderState);

            originProvider.SetPosition(Point.Empty);

            Thread.Sleep(waitBeforeScreenshots_);

            Bitmap initialScreenshot = imageProvider_.GetImage();
            Size initialPhysicalSize = initialScreenshot.Size;

            SaveDebugScreenshotPart_(initialScreenshot, region.ToRectangle(), "initial");

            IScaleProvider scaleProvider = scaleProviderFactory_.GetScaleProvider(initialScreenshot.Width);
            double pixelRatio = 1 / scaleProvider.ScaleRatio;

            Size initialSizeScaled = new Size((int)Math.Round(initialScreenshot.Width / pixelRatio), (int)Math.Round(initialScreenshot.Height / pixelRatio));

            ICutProvider scaledCutProvider = cutProvider_.Scale(pixelRatio);
            if (pixelRatio != 1 && !(scaledCutProvider is NullCutProvider))
            {
                initialScreenshot = cutProvider_.Cut(initialScreenshot);
                debugScreenshotsProvider_.Save(initialScreenshot, "original-cut");
            }

            Region regionInScreenshot = GetRegionInScreenshot_(region, initialScreenshot, pixelRatio);
            Bitmap croppedInitialScreenshot = CropScreenshot_(initialScreenshot, regionInScreenshot);
            debugScreenshotsProvider_.Save(croppedInitialScreenshot, "cropped");

            Bitmap scaledInitialScreenshot = BasicImageUtils.ScaleImage(croppedInitialScreenshot, scaleProvider);
            if (!object.ReferenceEquals(scaledInitialScreenshot, croppedInitialScreenshot))
            {
                SaveDebugScreenshotPart_(scaledInitialScreenshot, regionInScreenshot.ToRectangle(), "scaled");
            }

            if (fullarea.IsEmpty)
            {
                Size entireSize;
                try
                {
                    entireSize = positionProvider.GetEntireSize();
                    logger_.Verbose("Entire size of region context: {0}", entireSize);
                }
                catch (EyesException e)
                {
                    logger_.Log("WARNING: Failed to extract entire size of region context" + e.Message);
                    logger_.Log("Using image size instead: " + scaledInitialScreenshot.Width + "x" + scaledInitialScreenshot.Height);
                    entireSize = new Size(scaledInitialScreenshot.Width, scaledInitialScreenshot.Height);
                }

                // Notice that this might still happen even if we used
                // "getImagePart", since "entirePageSize" might be that of a frame.
                if (scaledInitialScreenshot.Width >= entireSize.Width && scaledInitialScreenshot.Height >= entireSize.Height)
                {
                    logger_.Log("WARNING: Seems the image is already a full page screenshot.");
                    if (!object.ReferenceEquals(scaledInitialScreenshot, initialScreenshot))
                    {
                        initialScreenshot.Dispose();
                    }
                    return scaledInitialScreenshot;
                }

                fullarea = new Region(Point.Empty, entireSize, CoordinatesTypeEnum.SCREENSHOT_AS_IS);
            }

            float currentFullWidth = fullarea.Width;
            fullarea = sizeAdjuster_.AdjustRegion(fullarea, initialSizeScaled);
            float sizeRatio = currentFullWidth / fullarea.Width;
            logger_.Verbose("adjusted fullarea: {0}", fullarea);

            Point scaledCropLocation = fullarea.Location;

            Point physicalCropLocation = new Point(
                (int)Math.Ceiling(scaledCropLocation.X * pixelRatio),
                (int)Math.Ceiling(scaledCropLocation.Y * pixelRatio));

            Rectangle sourceRegion;
            if (regionInScreenshot.IsSizeEmpty)
            {
                Size physicalCropSize = new Size(initialPhysicalSize.Width - physicalCropLocation.X, initialPhysicalSize.Height - physicalCropLocation.Y);
                sourceRegion = new Rectangle(physicalCropLocation, physicalCropSize);
            }
            else
            {
                // Starting with the screenshot we already captured at (0,0).
                sourceRegion = regionInScreenshot.ToRectangle();
            }

            Rectangle scaledCroppedSourceRect = cutProvider_.ToRectangle(sourceRegion.Size);
            scaledCroppedSourceRect.Offset(sourceRegion.Location);
            Rectangle scaledCroppedSourceRegion = new Rectangle(
                (int)Math.Ceiling(scaledCroppedSourceRect.X / pixelRatio),
                (int)Math.Ceiling(scaledCroppedSourceRect.Y / pixelRatio),
                (int)Math.Ceiling(scaledCroppedSourceRect.Width / pixelRatio),
                (int)Math.Ceiling(scaledCroppedSourceRect.Height / pixelRatio));

            Size scaledCropSize = scaledCroppedSourceRegion.Size;

            // The screenshot part is a bit smaller than the screenshot size, in order to eliminate
            // duplicate bottom/right-side scroll bars, as well as fixed position footers.
            Size screenshotPartSize = new Size(
                Math.Max(scaledCropSize.Width, MinScreenshotPartSize_),
                Math.Max(scaledCropSize.Height, MinScreenshotPartSize_)
                );

            logger_.Verbose("Screenshot part size: {0}", screenshotPartSize);

            // Getting the list of viewport regions composing the page (we'll take screenshot for each one).
            Rectangle rectInScreenshot;
            if (regionInScreenshot.IsSizeEmpty)
            {
                int x = Math.Max(0, fullarea.Left);
                int y = Math.Max(0, fullarea.Top);
                int w = Math.Min(fullarea.Width, scaledCropSize.Width);
                int h = Math.Min(fullarea.Height, scaledCropSize.Height);
                rectInScreenshot = new Rectangle(
                    (int)Math.Round(x * pixelRatio),
                    (int)Math.Round(y * pixelRatio),
                    (int)Math.Round(w * pixelRatio),
                    (int)Math.Round(h * pixelRatio));
            }
            else
            {
                rectInScreenshot = regionInScreenshot.Rectangle;
            }

            fullarea = CoerceImageSize_(fullarea);

            ICollection<SubregionForStitching> screenshotParts = fullarea.GetSubRegions(screenshotPartSize, stitchOverlap_, pixelRatio, rectInScreenshot, logger_);

            Bitmap stitchedImage = new Bitmap(fullarea.Width, fullarea.Height);
            // Take screenshot and stitch for each screenshot part.
            StitchScreenshot_(stitchOffset, positionProvider, screenshotParts, stitchedImage, scaleProvider.ScaleRatio, scaledCutProvider, sizeRatio);

            positionProvider.SetPosition(originalStitchedState);
            originProvider.RestoreState(originProviderState);

            croppedInitialScreenshot.Dispose();
            return stitchedImage;
        }

        private Region CoerceImageSize_(Region fullarea)
        {
            if (fullarea.Height < maxHeight_ && fullarea.Area < maxArea_)
            {
                logger_.Verbose("full area fits server limits.");
                return fullarea;
            }

            if (maxArea_ == 0 || maxHeight_ == 0)
            {
                logger_.Verbose("server limits unspecified.");
                return fullarea;
            }

            int trimmedHeight = Math.Min(maxArea_ / fullarea.Width, maxHeight_);
            Region newRegion = new Region(fullarea.Left, fullarea.Top, fullarea.Width, trimmedHeight, fullarea.CoordinatesType);
            if (newRegion.IsSizeEmpty)
            {
                logger_.Verbose("empty region after coerce. returning original.");
                return fullarea;
            }
            logger_.Verbose("coerced region: {0}", newRegion);
            return newRegion;
        }

        private Region GetRegionInScreenshot_(Region region, Bitmap image, double pixelRatio)
        {
            if (region.IsSizeEmpty)
            {
                return region;
            }

            logger_.Verbose("Creating screenshot object...");
            // We need the screenshot to be able to convert the region to screenshot coordinates.
            EyesScreenshot screenshot = getEyesScreenshot_(image);
            logger_.Verbose("Getting region in screenshot...");

            // Region regionInScreenshot = screenshot.convertRegionLocation(regionProvider.getRegion(), regionProvider.getCoordinatesType(), CoordinatesType.SCREENSHOT_AS_IS);
            Region regionInScreenshot = screenshot.GetIntersectedRegion(region, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

            Size scaledImageSize = new Size((int)Math.Round(image.Width / pixelRatio), (int)Math.Round(image.Height / pixelRatio));
            regionInScreenshot = sizeAdjuster_.AdjustRegion(regionInScreenshot, scaledImageSize);

            logger_.Verbose("Region in screenshot: {0}", regionInScreenshot);
            regionInScreenshot = regionInScreenshot.Scale(pixelRatio);
            logger_.Verbose("Scaled region: {0}", regionInScreenshot);

            regionInScreenshot = regionPositionCompensation_.CompensateRegionPosition(regionInScreenshot, pixelRatio);

            // Handling a specific case where the region is actually larger than
            // the screenshot (e.g., when body width/height are set to 100%, and
            // an internal div is set to value which is larger than the viewport).
            regionInScreenshot.Intersect(new Region(0, 0, image.Width, image.Height));
            logger_.Verbose("Region after intersect: {0}", regionInScreenshot);
            return regionInScreenshot;
        }

        private Bitmap CropScreenshot_(Bitmap initialScreenshot, Region regionInScreenshot)
        {
            if (!regionInScreenshot.IsSizeEmpty)
            {
                Bitmap croppedInitialScreenshot = BasicImageUtils.Crop(initialScreenshot, regionInScreenshot.ToRectangle());
                initialScreenshot.Dispose();
                initialScreenshot = croppedInitialScreenshot;
                SaveDebugScreenshotPart_(croppedInitialScreenshot, regionInScreenshot.ToRectangle(), "cropped");
            }
            return initialScreenshot;
        }

        /// <summary>
        /// Encapsulates an algorithm for creating full-page images of a page.
        /// </summary>
        /// <param name="positionProvider">The position provider used for stitching.</param>
        /// <returns>The screenshot as PNG byte array.</returns>
        public byte[] GetFullPageScreenshot(IPositionProvider positionProvider)
        {
            using (Bitmap screenshotImage = GetStitchedRegion(Region.Empty, Region.Empty, positionProvider, positionProvider))
            {
                return BasicImageUtils.EncodeAsPng(screenshotImage);
            }
        }

        private void StitchScreenshot_(Size stitchOffset, IPositionProvider stitchProvider,
            ICollection<SubregionForStitching> screenshotParts, Bitmap stitchedImage, double scaleRatio,
            ICutProvider scaledCutProvider, float sizeRatio)
        {
            int index = 0;
            logger_.Verbose($"enter: {nameof(stitchOffset)}: {{0}} ; {nameof(screenshotParts)}.Count: {{1}}, {nameof(scaleRatio)}: {{2}}",
                stitchOffset, screenshotParts.Count, scaleRatio);

            Stopwatch stopwatch = Stopwatch.StartNew();
            foreach (SubregionForStitching partRegion in screenshotParts)
            {
                if (stopwatch.Elapsed > TimeSpan.FromMinutes(5))
                {
                    logger_.Log("Still Running..."); // this is so CI systems won't kill the build due to lack of activity.
                    stopwatch.Restart();
                }

                logger_.Verbose("Part: {0}", partRegion);
                // Scroll to the part's top/left
                Point partAbsoluteLocationInCurrentFrame = partRegion.ScrollTo;
                partAbsoluteLocationInCurrentFrame += stitchOffset;
                Point scrollPosition = new Point(
                    (int)Math.Round(partAbsoluteLocationInCurrentFrame.X * sizeRatio),
                    (int)Math.Round(partAbsoluteLocationInCurrentFrame.Y * sizeRatio));
                Point originPosition = stitchProvider.SetPosition(scrollPosition);

                int dx = scrollPosition.X - originPosition.X;
                int dy = scrollPosition.Y - originPosition.Y;

                Point partPastePosition = partRegion.PasteLocation;
                //partPastePosition.Offset(-fullarea.Left, -fullarea.Top);
                partPastePosition.Offset(-dx, -dy);

                // Actually taking the screenshot.
                Thread.Sleep(waitBeforeScreenshots_);
                using (Bitmap partImage = imageProvider_.GetImage())
                using (Bitmap cutPart = scaledCutProvider.Cut(partImage))
                {
                    Bitmap croppedPart;
                    Rectangle r = partRegion.PhysicalCropArea;
                    r.Width += dx;
                    r.Height += dy;
                    if ((r.Width * r.Height) != 0)
                    {
                        croppedPart = BasicImageUtils.Crop(cutPart, r);
                    }
                    else
                    {
                        croppedPart = cutPart;
                    }

                    Rectangle r2 = partRegion.LogicalCropArea;
                    r2.Width += dx;
                    r2.Height += dy;

                    using (Bitmap scaledPartImage = BasicImageUtils.ScaleImage(croppedPart, scaleRatio))
                    using (Bitmap scaledCroppedPartImage = BasicImageUtils.Crop(scaledPartImage, r2))
                    using (Graphics g = Graphics.FromImage(stitchedImage))
                    {
                        debugScreenshotsProvider_.Save(partImage, "partImage-" + originPosition.X + "_" + originPosition.Y);
                        debugScreenshotsProvider_.Save(cutPart, "cutPart-" + originPosition.X + "_" + originPosition.Y);
                        debugScreenshotsProvider_.Save(croppedPart, "croppedPart-" + originPosition.X + "_" + originPosition.Y);
                        debugScreenshotsProvider_.Save(scaledPartImage, "scaledPartImage-" + originPosition.X + "_" + originPosition.Y);
                        debugScreenshotsProvider_.Save(scaledCroppedPartImage, "scaledCroppedPartImage-" + partPastePosition.X + "_" + partPastePosition.Y);
                        logger_.Verbose("pasting part at {0}", partPastePosition);
                        g.DrawImage(scaledCroppedPartImage, partPastePosition);
                    }
                    if (!object.ReferenceEquals(croppedPart, cutPart))
                    {
                        croppedPart.Dispose();
                    }
                    debugScreenshotsProvider_.Save(stitchedImage, $"stitched_{index}_({partPastePosition.X}_{partPastePosition.Y})");
                    index++;
                }
            }

            debugScreenshotsProvider_.Save(stitchedImage, "stitched");
        }

    }
}
