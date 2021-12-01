using System;
using System.Collections.Generic;
using System.Drawing;
using Applitools.Utils;
using Applitools.Utils.Images;
using OpenQA.Selenium;

namespace Applitools.Selenium.Capture
{
    internal class SafariScreenshotImageProvider : MobileScreenshotImageProvider
    {
        private const string defaultiOSKey = "*";
        private static IDictionary<string, IDictionary<Size, Rectangle[]>> regionsPeriOsVersion_;

        public SafariScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
            : base(eyes, logger, tsInstance, userAgent)
        {
        }

        public override Bitmap GetImage()
        {
            Bitmap image = base.GetImage();

            if (eyes_.CachedSessionDetails != null &&
                eyes_.CachedSessionDetails.TryGetValue("deviceOrientation", out object orientation))
            {
                if ("landscape".Equals(orientation as string, StringComparison.OrdinalIgnoreCase) && image.Width < image.Height)
                {
                    logger_.Verbose("rotating image...");
                    image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                }
            }

            logger_.Verbose("Bitmap Size: {0}x{1}", image.Width, image.Height);
            eyes_.DebugScreenshotProvider.Save(image, "SAFARI");

            if (eyes_.IsCutProviderExplicitlySet)
            {
                return image;
            }

            double scaleRatio = eyes_.DevicePixelRatio;

            Size originalViewportSize = GetViewportSize();

            Size viewportSize = new Size(
                (int)Math.Ceiling(originalViewportSize.Width * scaleRatio),
                (int)Math.Ceiling(originalViewportSize.Height * scaleRatio));

            logger_.Verbose("logical viewport size: " + originalViewportSize);

            Bitmap croppedImage = null;
            if (userAgent_.IsiOS ||
                "ios".Equals(eyes_.PlatformName, StringComparison.OrdinalIgnoreCase) ||
                (eyes_.CachedSessionDetails.TryGetValue("PlatformName", out object platformName) &&
                    "ios".Equals(platformName as string, StringComparison.OrdinalIgnoreCase)))
            {
                croppedImage = CropIOSImage(image, originalViewportSize, userAgent_.BrowserMajorVersion, logger_);
            }
            else if (!eyes_.ForceFullPageScreenshot)
            {
                Point loc;
                FrameChain currentFrameChain = eyes_.GetDriver().GetFrameChain();

                if (currentFrameChain.Count == 0)
                {
                    IWebElement scrollRootElement = eyes_.GetCurrentFrameScrollRootElement();
                    IPositionProvider positionProvider = eyes_.GetPositionProvider(
                        logger_, StitchModes.Scroll, jsExecutor_, scrollRootElement,
                        userAgent_);
                    loc = positionProvider.GetCurrentPosition();
                }
                else
                {
                    loc = currentFrameChain.GetDefaultContentScrollPosition();
                }

                loc = new Point((int)Math.Ceiling(loc.X * scaleRatio), (int)Math.Ceiling(loc.Y * scaleRatio));

                croppedImage = BasicImageUtils.Crop(image, new Rectangle(loc, viewportSize));
            }
            if (croppedImage != null && !ReferenceEquals(croppedImage, image))
            {
                image.Dispose();
                image = croppedImage;
            }
            eyes_.DebugScreenshotProvider.Save(image, "SAFARI_CROPPED");

            return image;
        }

        internal static Bitmap CropIOSImage(Bitmap image, Size originalViewportSize, Logger logger = null)
        {
            return CropIOSImage(image, originalViewportSize, defaultiOSKey, logger);
        }

        internal static Bitmap CropIOSImage(Bitmap image, Size originalViewportSize, string iosMajorVersion, Logger logger = null)
        {
            if (logger == null) logger = new Logger();

            if (regionsPeriOsVersion_ == null)
            {
                InitDeviceRegionsTable_();
            }

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            logger.Verbose("iOS version: {0}", iosMajorVersion);
            logger.Verbose("physical device pixel size: {0}x{1}", imageWidth, imageHeight);

            var devicesRegions = GetiOSRegions(iosMajorVersion);
            if (devicesRegions.TryGetValue(image.Size, out Rectangle[] resolutions))
            {
                int renderedWidth = resolutions[0].Width;
                int relevantViewportHeight = (renderedWidth < image.Width) ? originalViewportSize.Height - 21 : originalViewportSize.Height;
                float widthRatio = renderedWidth / (float)originalViewportSize.Width;
                float height = widthRatio * relevantViewportHeight;
                if (Math.Abs(height - image.Height) > 1.5)
                {
                    Rectangle bestMatchingRect = resolutions[0];
                    float bestHeightDiff = Math.Abs(bestMatchingRect.Height - height);
                    logger.Verbose("bestMatchingRect: {0} ; bestHeightDiff: {1}", bestMatchingRect, bestHeightDiff);
                    for (int i = 1; i < resolutions.Length; ++i)
                    {
                        Rectangle rect = resolutions[i];
                        float heightDiff = Math.Abs(rect.Height - height);
                        logger.Verbose("rect: {0} ; heightDiff: {1} ; bestHeightDiff: {2}", rect, heightDiff, bestHeightDiff);
                        if (heightDiff < bestHeightDiff)
                        {
                            bestHeightDiff = heightDiff;
                            bestMatchingRect = rect;
                            logger.Verbose("updated bestHeightDiff to {0} and bestMatchingRect to {1}", bestHeightDiff, bestMatchingRect);
                        }
                    }
                    logger.Verbose("closest crop rect found: {0}", bestMatchingRect);
                    image = BasicImageUtils.Crop(image, bestMatchingRect);
                }
                else
                {
                    logger.Verbose("no crop needed. must be using chrome emulator.");
                }
            }

            return image;
        }

        private static IDictionary<Size, Rectangle[]> GetiOSRegions(string iOSVersion)
        {
            if (regionsPeriOsVersion_.TryGetValue(iOSVersion, out var regions))
            {
                return regions;
            }

            return regionsPeriOsVersion_[defaultiOSKey];
        }

        private static void InitDeviceRegionsTable_()
        {
            regionsPeriOsVersion_ = new Dictionary<string, IDictionary<Size, Rectangle[]>>
            {
                {
                    defaultiOSKey, new Dictionary<Size, Rectangle[]>
                    {
                        {new Size(1536, 2048), new[] {new Rectangle(0, 140, 1536, 1908), new Rectangle(0, 205, 1536, 1843), new Rectangle(0, 128, 1536, 1920), new Rectangle(0, 194, 1536, 1854)}},
                        {new Size(2048, 1536), new[] {new Rectangle(0, 140, 2048, 1396), new Rectangle(0, 205, 2048, 1331), new Rectangle(0, 128, 2048, 1408), new Rectangle(0, 194, 2048, 1342)}},

                        {new Size(828, 1792), new[] {new Rectangle(0, 188, 828, 1438)}},
                        {new Size(1792, 828), new[] {new Rectangle(88, 100, 1616, 686), new Rectangle(88, 166, 1616, 620)}},

                        {new Size(1242, 2688), new[] {new Rectangle(0, 282, 1242, 2157)}},
                        {new Size(2688, 1242), new[] {new Rectangle(132, 150, 2424, 1029), new Rectangle(132, 249, 2424, 930)}},

                        {new Size(1125, 2436), new[] {new Rectangle(0, 282, 1125, 1905)}},
                        {new Size(2436, 1125), new[] {new Rectangle(132, 150, 2172, 912), new Rectangle(132, 249, 2172, 813)}},

                        {new Size(1242, 2208), new[] {new Rectangle(0, 210, 1242, 1866), new Rectangle(0, 192, 1242, 1884)}},
                        {new Size(2208, 1242), new[] {new Rectangle(0, 132, 2208, 1110), new Rectangle(0, 150, 2208, 1092), new Rectangle(0, 230, 2208, 1012)}},

                        {new Size(750, 1334), new[] {new Rectangle(0, 140, 750, 1106), new Rectangle(0, 128, 750, 1118)}},
                        {new Size(1334, 750), new[] {new Rectangle(0, 100, 1334, 650), new Rectangle(0, 88, 1334, 662)}},

                        {new Size(640, 1136), new[] {new Rectangle(0, 128, 640, 920)}},
                        {new Size(1136, 640), new[] {new Rectangle(0, 88, 1136, 464)}},

                        {new Size(2048, 2732), new[] {new Rectangle(0, 149, 2048, 2542), new Rectangle(0, 140, 2048, 2592)}},
                        {new Size(2732, 2048), new[] {new Rectangle(0, 140, 2732, 1908)}},

                        {new Size(1668, 2224), new[] {new Rectangle(0, 140, 1668, 2084)}},
                        {new Size(2224, 1668), new[] {new Rectangle(0, 140, 2224, 1528)}},

                        {new Size(1620, 2160), new[] {new Rectangle(0, 140, 1620, 2020)}},
                        {new Size(2160, 1620), new[] {new Rectangle(0, 140, 2160, 1480)}},

                        {new Size(1640, 2360), new[] {new Rectangle(0, 149, 1640, 2211)}},

                        // iPhone 12, iOS 14
                        {new Size(1170, 2532), new[] {new Rectangle(0, 291, 1170, 1991)}},
                        {new Size(2532, 1170), new[] {new Rectangle(141, 150, 2250, 981)}}
                    }
                },
                {
                    "15", new Dictionary<Size, Rectangle[]>
                    {
                        // iPhone 12, iOS 15
                        {new Size(1170, 2532), new[] {new Rectangle(0, 141, 1170, 1993)}},
                        {new Size(2532, 1170), new[] {new Rectangle(147, 150, 2238, 981)}}
                    }
                }
            };
        }
    }
}