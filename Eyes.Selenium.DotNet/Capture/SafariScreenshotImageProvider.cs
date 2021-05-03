using Applitools.Selenium.Scrolling;
using Applitools.Utils;
using Applitools.Utils.Images;
using OpenQA.Selenium;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Selenium.Capture
{
    internal class SafariScreenshotImageProvider : MobileScreenshotImageProvider
    {
        private static Dictionary<Size, List<Rectangle>> devicesRegions = null;

        public SafariScreenshotImageProvider(SeleniumEyes eyes, Logger logger, ITakesScreenshot tsInstance, UserAgent userAgent)
            : base(eyes, logger, tsInstance, userAgent)
        {
        }

        public override Bitmap GetImage()
        {
            Bitmap image = base.GetImage();
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
            if (userAgent_.OS.Equals(OSNames.IOS))
            {
                croppedImage = CropIOSImage(image, originalViewportSize, logger_);
            }
            else if (!eyes_.ForceFullPageScreenshot)
            {
                Point loc;
                FrameChain currentFrameChain = eyes_.GetDriver().GetFrameChain();

                if (currentFrameChain.Count == 0)
                {
                    IWebElement scrollRootElement = eyes_.GetCurrentFrameScrollRootElement();
                    IPositionProvider positionProvider = SeleniumPositionProviderFactory.GetPositionProvider(
                        logger_, StitchModes.Scroll, jsExecutor_, eyes_.GetDriver().RemoteWebDriver,
                        scrollRootElement, userAgent_);
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
            if (logger == null) logger = new Logger();

            if (devicesRegions == null)
            {
                InitDeviceRegionsTable_();
            }

            int imageWidth = image.Width;
            int imageHeight = image.Height;

            logger.Verbose("physical device pixel size: {0}x{1}", imageWidth, imageHeight);

            if (devicesRegions.TryGetValue(image.Size, out List<Rectangle> resolutions))
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
                    for (int i = 1; i < resolutions.Count; ++i)
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

        private static void InitDeviceRegionsTable_()
        {
            devicesRegions = new Dictionary<Size, List<Rectangle>>
            {
                { new Size(1536, 2048), new List<Rectangle>{ new Rectangle(0, 140, 1536, 1908), new Rectangle(0, 205, 1536, 1843), new Rectangle(0, 128, 1536, 1920), new Rectangle(0, 194, 1536, 1854) } },
                { new Size(2048, 1536), new List<Rectangle>{ new Rectangle(0, 140, 2048, 1396), new Rectangle(0, 205, 2048, 1331), new Rectangle(0, 128, 2048, 1408), new Rectangle(0, 194, 2048, 1342) } },

                { new Size(828, 1792), new List<Rectangle>{ new Rectangle(0, 188, 828, 1438) } },
                { new Size(1792, 828), new List<Rectangle>{ new Rectangle(88, 100, 1616, 686), new Rectangle(88, 166, 1616, 620) } },

                { new Size(1242, 2688), new List<Rectangle>{ new Rectangle(0, 282, 1242, 2157) } },
                { new Size(2688, 1242), new List<Rectangle>{ new Rectangle(132, 150, 2424, 1029), new Rectangle(132, 249, 2424, 930) } },

                { new Size(1125, 2436), new List<Rectangle>{ new Rectangle(0, 282, 1125, 1905) } },
                { new Size(2436, 1125), new List<Rectangle>{ new Rectangle(132, 150, 2172, 912), new Rectangle(132, 249, 2172, 813) } },

                { new Size(1242, 2208), new List<Rectangle>{ new Rectangle(0, 210, 1242, 1866), new Rectangle(0, 192, 1242, 1884) } },
                { new Size(2208, 1242), new List<Rectangle>{ new Rectangle(0, 132, 2208, 1110), new Rectangle(0, 150, 2208, 1092), new Rectangle(0, 230, 2208, 1012) } },

                { new Size(750, 1334), new List<Rectangle>{ new Rectangle(0, 140, 750, 1106), new Rectangle(0, 128, 750, 1118) } },
                { new Size(1334, 750), new List<Rectangle>{ new Rectangle(0, 100, 1334, 650), new Rectangle(0, 88, 1334, 662) } },

                { new Size(640, 1136), new List<Rectangle>{ new Rectangle(0, 128, 640, 920) } },
                { new Size(1136, 640), new List<Rectangle>{ new Rectangle(0, 88, 1136, 464) } },

                { new Size(2048, 2732), new List<Rectangle>{ new Rectangle(0, 140, 2048, 2592) } },
                { new Size(2732, 2048), new List<Rectangle>{ new Rectangle(0, 140, 2732, 1908) } },

                { new Size(1668, 2224), new List<Rectangle>{ new Rectangle(0, 140, 1668, 2084) } },
                { new Size(2224, 1668), new List<Rectangle>{ new Rectangle(0, 140, 2224, 1528) } }
            };
        }
    }
}