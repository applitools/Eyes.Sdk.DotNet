using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;

namespace Applitools.Utils.Geometry
{
    /// <summary>
    /// A rectangular region.
    /// </summary>
    public struct Region : IRegion
    {
        /// <summary>
        /// An empty region.
        /// </summary>
        public static readonly Region Empty = new Region(0, 0, 0, 0, CoordinatesTypeEnum.SCREENSHOT_AS_IS);

        /// <summary>
        /// Create a new <see cref="Region"/> instance.
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <param name="coordinateType"></param>
        public Region(int left, int top, int width, int height, CoordinatesTypeEnum coordinateType = CoordinatesTypeEnum.SCREENSHOT_AS_IS)
        {
            ArgumentGuard.GreaterOrEqual(width, 0, nameof(width));
            ArgumentGuard.GreaterOrEqual(height, 0, nameof(height));

            Left = left;
            Top = top;
            Width = width;
            Height = height;
            CoordinatesType = coordinateType;
        }

        /// <summary>
        /// Create a new <see cref="Region"/> instance.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="size"></param>
        /// <param name="coordinateType"></param>
        public Region(Point position, Size size, CoordinatesTypeEnum coordinateType = CoordinatesTypeEnum.SCREENSHOT_AS_IS)
            : this(position.X, position.Y, size.Width, size.Height, coordinateType)
        {
        }

        /// <summary>
        /// Create a new <see cref="Region"/> instance.
        /// </summary>
        /// <param name="rectangle"></param>
        /// <param name="coordinateType"></param>
        public Region(Rectangle rectangle, CoordinatesTypeEnum coordinateType = CoordinatesTypeEnum.SCREENSHOT_AS_IS)
            : this(rectangle.Left, rectangle.Top, rectangle.Width, rectangle.Height, coordinateType)
        {
        }

        /// <summary>
        /// Copy constructor for creating an immutable region from other regions.
        /// </summary>
        /// <param name="region">Some region. Can be <see cref="MutableRegion"/>.</param>
        public Region(IRegion region) : this(region.Rectangle) { }

        /// <summary>
        /// Returns the location of this <see cref="Region"/> on the X axis.
        /// </summary>
        public int Left { get; private set; }
        /// <summary>
        /// Returns the location of this <see cref="Region"/> on the Y axis.
        /// </summary>
        public int Top { get; private set; }

        /// <summary>
        /// Sysnonym for <see cref="Left"/>. Used for deserialization.
        /// </summary>
        public int X { set => Left = value; }

        /// <summary>
        /// Sysnonym for <see cref="Top"/>. Used for deserialization.
        /// </summary>
        public int Y { set => Top = value; }

        /// <summary>
        /// Returns the width component of the size of this <see cref="Region"/>.
        /// </summary>
        public int Width { get; private set; }

        /// <summary>
        /// Returns the height component of the size of this <see cref="Region"/>.
        /// </summary>
        public int Height { get; private set; }

        /// <summary>
        /// Returns whether this <see cref="Region"/> is an empty one, by comparing both Size and Location to the <see cref="Region.Empty"/> instance.
        /// </summary>
        [JsonIgnore]
        public bool IsEmpty
        {
            get
            {
                return this.Left == Empty.Left && this.Top == Empty.Top && this.Width == Empty.Width && this.Height == Empty.Height;
            }
        }

        /// <summary>
        /// Returns whether both the width and the height of this <see cref="Region"/> are zero (0).
        /// </summary>
        [JsonIgnore]
        public bool IsSizeEmpty
        {
            get { return this.Width == 0 || this.Height == 0; }
        }

        private void MakeEmpty_()
        {
            Left = Empty.Left;
            Top = Empty.Top;
            Width = Empty.Width;
            Height = Empty.Height;
        }

        /// <summary>
        /// Return the <see cref="CoordinatesTypeEnum"/> of this <see cref="Region"/>.
        /// </summary>
        public CoordinatesTypeEnum CoordinatesType { get; private set; }

        /// <returns>This region represented as a <see cref="Rectangle"/></returns>
        public Rectangle ToRectangle()
        {
            return new Rectangle(Left, Top, Width, Height);
        }

        /// <summary>
        /// Returns the size of this <see cref="Region"/>.
        /// </summary>
        [JsonIgnore]
        public Size Size { get { return new Size(Width, Height); } }

        /// <summary>
        /// Returns the location of this <see cref="Region"/>.
        /// </summary>
        [JsonIgnore]
        public Point Location { get { return new Point(Left, Top); } }

        [JsonIgnore]
        public int Right => Left + Width;

        [JsonIgnore]
        public int Bottom => Top + Height;

        [JsonIgnore]
        public int Area => Width * Height;

        [JsonIgnore]
        public Rectangle Rectangle => ToRectangle();

        /// <summary>
        /// Get a region which is a scaled version of the current region.
        /// IMPORTANT: This also scales the LOCATION of the region (not just its size).
        /// </summary>
        /// <param name="scaleRatio">The ratio by which to scale the region.</param>
        /// <returns>A new <see cref="Region"/> which is a scaled version of the current region.</returns>
        public Region Scale(double scaleRatio)
        {
            return new Region(
                (int)Math.Ceiling(Left * scaleRatio),
                (int)Math.Ceiling(Top * scaleRatio),
                (int)Math.Round(Width * scaleRatio),
                (int)Math.Round(Height * scaleRatio));
        }

        /// <summary>
        /// Gets the center of this region.
        /// </summary>
        public Location GetCenter()
        {
            return new Location(Left + (Width / 2), Top + (Height / 2));
        }

        /// <summary>
        /// Check if a region is contained within the current region.
        /// </summary>
        /// <param name="other">The region to check if it is contained within the current region.</param>
        /// <returns>True if <paramref name="other"/> is contained within the current region, false otherwise.</returns>
        public bool Contains(Region other)
        {
            int right = Left + Width;
            int otherRight = other.Left + other.Width;

            int bottom = Top + Height;
            int otherBottom = other.Top + other.Height;

            return Top <= other.Top && Left <= other.Left
                    && bottom >= otherBottom && right >= otherRight;
        }

        /// <summary>
        /// Offsets a clone of this region instance by given amount of pixels.
        /// </summary>
        /// <param name="x">Amount of pixels to move the region horizontally.</param>
        /// <param name="y">Amount of pixels to move the region vertically.</param>
        /// <returns>A clone of this region, offseted by the given amount of pixels.</returns>
        public Region Offset(int x, int y)
        {
            return new Region(Left + x, Top + y, Width, Height, CoordinatesType);
        }

        /// <summary>
        /// Check if a specified location is contained within this region.
        /// </summary>
        /// <param name="location">The location to test.</param>
        /// <returns>True if <paramref name="location"/> is contained within this region, false otherwise.</returns>
        public bool Contains(Point location)
        {
            return location.X >= Left &&
                   location.X <= (Left + Width) &&
                   location.Y >= Top &&
                   location.Y <= (Top + Height);
        }

        public Region RemoveBorders(RectangularMargins borders)
        {
            return new Region(
                Left + borders.Left, Top + borders.Top,
                Width - borders.Horizontal, Height - borders.Vertical,
                CoordinatesType);
        }

        public static Rectangle RemoveBorders(Rectangle rectangle, RectangularMargins borders)
        {
            return new Rectangle(
                rectangle.Left + borders.Left, rectangle.Top + borders.Top,
                rectangle.Width - borders.Horizontal, rectangle.Height - borders.Vertical);
        }

        /// <summary>
        /// Returns sub-regions composing the current region.
        /// </summary>
        /// <param name="maxSubRegionSize">The maximum size of each sub-region (regions might be
        /// smaller).
        /// </param>
        /// <returns>The sub-regions composing the current region. <code>maxSubRegionSize</code> 
        /// is equal or greater than the current region, only a single region is returned.
        /// </returns>
        public ICollection<Region> GetSubRegions(Size maxSubRegionSize)
        {
            var subRegions = new List<Region>();

            int currentTop = Top;
            int bottom = Top + Height;
            int right = Left + Width;

            while (currentTop < bottom)
            {
                int currentBottom = currentTop + maxSubRegionSize.Height;
                if (currentBottom > bottom)
                {
                    currentBottom = bottom;
                }

                int currentLeft = Left;
                while (currentLeft < right)
                {
                    int currentRight = currentLeft + maxSubRegionSize.Width;
                    if (currentRight > right)
                    {
                        currentRight = right;
                    }

                    int currentHeight = currentBottom - currentTop;
                    int currentWidth = currentRight - currentLeft;

                    subRegions.Add(
                        new Region(currentLeft, currentTop, currentWidth, currentHeight));

                    currentLeft += maxSubRegionSize.Width;
                }

                currentTop += maxSubRegionSize.Height;
            }

            return subRegions;
        }

        public ICollection<SubregionForStitching> GetSubRegions(Size maxSubRegionSize, int logicalOverlap, double l2pScaleRatio, Rectangle physicalRectInScreenshot, Logger logger)
        {
            logger.Verbose($"{nameof(maxSubRegionSize)}: {{0}} ; {nameof(logicalOverlap)}: {{1}} ; logical to physical scaleRatio: {{2}} ; physicalRectInScreenshot: {{3}} ; this: {{4}}",
                maxSubRegionSize, logicalOverlap, l2pScaleRatio, physicalRectInScreenshot, this);

            ArgumentGuard.GreaterThan(physicalRectInScreenshot.Width, 0, nameof(physicalRectInScreenshot) + ".Width");
            ArgumentGuard.GreaterThan(physicalRectInScreenshot.Height, 0, nameof(physicalRectInScreenshot) + ".Height");

            var subRegions = new List<SubregionForStitching>();

            int doubleLogicalOverlap = logicalOverlap * 2;
            int physicalOverlap = (int)Math.Round(doubleLogicalOverlap * l2pScaleRatio);

            bool needVScroll = (Height * l2pScaleRatio) > physicalRectInScreenshot.Height;
            bool needHScroll = (Width * l2pScaleRatio) > physicalRectInScreenshot.Width;

            int scrollY = 0;
            int currentTop = 0;
            int currentLogicalHeight = maxSubRegionSize.Height;

            int deltaY = currentLogicalHeight - doubleLogicalOverlap;

            bool isTopEdge = true;
            bool isBottomEdge = false;

            while (!isBottomEdge)
            {
                int currentScrollTop = scrollY + maxSubRegionSize.Height;
                if (currentScrollTop >= Height)
                {
                    if (!isTopEdge)
                    {
                        scrollY = Height - currentLogicalHeight;
                        currentLogicalHeight = Height - currentTop;
                        currentTop = Height - currentLogicalHeight - doubleLogicalOverlap - logicalOverlap;
                    }
                    else
                    {
                        currentLogicalHeight = Height - currentTop;
                    }
                    isBottomEdge = true;
                }

                int scrollX = 0;
                int currentLeft = 0;
                int currentLogicalWidth = maxSubRegionSize.Width;

                int deltaX = currentLogicalWidth - doubleLogicalOverlap;

                bool isLeftEdge = true;
                bool isRightEdge = false;

                while (!isRightEdge)
                {
                    int currentScrollRight = scrollX + maxSubRegionSize.Width;
                    if (currentScrollRight >= Width)
                    {
                        if (!isLeftEdge)
                        {
                            scrollX = Width - currentLogicalWidth;
                            currentLogicalWidth = Width - currentLeft;
                            currentLeft = Width - currentLogicalWidth - doubleLogicalOverlap - logicalOverlap;
                        }
                        else
                        {
                            currentLogicalWidth = Width - currentLeft;
                        }
                        isRightEdge = true;
                    }

                    Rectangle physicalCropArea = physicalRectInScreenshot;
                    Rectangle logicalCropArea = new Rectangle(0, 0, currentLogicalWidth, currentLogicalHeight);
                    Point pastePoint = new Point(currentLeft, currentTop);

                    // handle horizontal
                    if (isRightEdge)
                    {
                        int physicalWidth = (int)Math.Round(currentLogicalWidth * l2pScaleRatio);
                        physicalCropArea.X = physicalRectInScreenshot.Right - physicalWidth;
                        physicalCropArea.Width = physicalWidth;
                    }

                    if (!isLeftEdge)
                    {
                        logicalCropArea.X += logicalOverlap;
                        logicalCropArea.Width -= logicalOverlap;
                    }

                    if (isRightEdge && !isLeftEdge)
                    {
                        // If scrolled to the right edge, make sure the left part is still inside physical viewport.
                        int newX = physicalCropArea.X - (physicalOverlap * 2);
                        if (newX >= physicalRectInScreenshot.Left) // everything is okay
                        {
                            physicalCropArea.X -= physicalOverlap * 2;
                            physicalCropArea.Width += physicalOverlap * 2;
                            logicalCropArea.Width += doubleLogicalOverlap * 2;
                        }
                        else // Oops, overshoot. We need to correct the width and left position.
                        {
                            int pDelta = physicalRectInScreenshot.Left - newX;
                            int lDelta = (int)Math.Round(pDelta / l2pScaleRatio);
                            physicalCropArea.X = physicalRectInScreenshot.Left;
                            physicalCropArea.Width += (physicalOverlap * 2) - pDelta;
                            logicalCropArea.Width += (doubleLogicalOverlap * 2) - lDelta;
                            pastePoint.X += lDelta;
                        }
                    }

                    // handle vertical
                    if (isBottomEdge)
                    {
                        int physicalHeight = (int)Math.Round(currentLogicalHeight * l2pScaleRatio);
                        physicalCropArea.Y = physicalRectInScreenshot.Bottom - physicalHeight;
                        physicalCropArea.Height = physicalHeight;
                    }

                    if (!isTopEdge)
                    {
                        logicalCropArea.Y += logicalOverlap;
                        logicalCropArea.Height -= logicalOverlap;
                    }

                    if (isBottomEdge && !isTopEdge)
                    {
                        // If scrolled to the bottom edge, make sure the top part is still inside physical viewport.
                        int newY = physicalCropArea.Y - (physicalOverlap * 2);
                        if (newY >= physicalRectInScreenshot.Top) // everything is okay
                        {
                            physicalCropArea.Y -= physicalOverlap * 2;
                            physicalCropArea.Height += physicalOverlap * 2;
                            logicalCropArea.Height += doubleLogicalOverlap * 2;
                        }
                        else // Oops, overshoot. We need to correct the height and top position.
                        {
                            int pDelta = physicalRectInScreenshot.Top - newY;
                            int lDelta = (int)Math.Round(pDelta / l2pScaleRatio);
                            physicalCropArea.Y = physicalRectInScreenshot.Top;
                            physicalCropArea.Height += (physicalOverlap * 2) - pDelta;
                            logicalCropArea.Height += (doubleLogicalOverlap * 2) - lDelta;
                            pastePoint.Y += lDelta;
                        }
                    }

                    SubregionForStitching subregion = new SubregionForStitching(
                        new Point(scrollX, scrollY),
                        pastePoint,
                        physicalCropArea,
                        logicalCropArea
                        );

                    logger.Verbose("adding subregion - {0}", subregion);

                    subRegions.Add(subregion);

                    currentLeft += deltaX;
                    scrollX += deltaX;

                    if (needHScroll && isLeftEdge)
                    {
                        currentLeft += logicalOverlap;
                    }
                    isLeftEdge = false;
                }

                currentTop += deltaY;
                scrollY += deltaY;

                if (needVScroll && isTopEdge)
                {
                    currentTop += logicalOverlap;
                }
                isTopEdge = false;
            }

            return subRegions;
        }

        /// <summary>
        /// Check if a region is intersected with the current region.
        /// </summary>
        /// <param name="other">The region to check intersection with.</param>
        /// <returns>True if the regions are intersected, false otherwise.</returns>
        public bool IsIntersected(Region other)
        {
            int right = Left + Width;
            int bottom = Top + Height;

            int otherLeft = other.Left;
            int otherTop = other.Top;
            int otherRight = otherLeft + other.Width;
            int otherBottom = otherTop + other.Height;

            return (((Left <= otherLeft && otherLeft <= right)
                        || (otherLeft <= Left && Left <= otherRight))
                    && ((Top <= otherTop && otherTop <= bottom)
                        || (otherTop <= Top && Top <= otherBottom)));
        }

        /// <summary>
        /// Replaces this region with the intersection of itself and <paramref name="other"/>
        /// </summary>
        /// <param name="other">The region with which to intersect.</param>
        public void Intersect(Region other)
        {
            // If there's no intersection set this as the Empty region.
            if (!IsIntersected(other))
            {
                MakeEmpty_();
                return;
            }

            // The regions intersect. So let's first find the left & top values
            int otherLeft = other.Left;
            int otherTop = other.Top;

            int intersectionLeft = (Left >= otherLeft) ? Left : otherLeft;
            int intersectionTop = (Top >= otherTop) ? Top : otherTop;

            // Now the width and height of the intersect
            int right = Left + Width;
            int otherRight = otherLeft + other.Width;
            int intersectionRight = (right <= otherRight) ? right : otherRight;
            int intersectionWidth = intersectionRight - intersectionLeft;

            int bottom = Top + Height;
            int otherBottom = otherTop + other.Height;
            int intersectionBottom = (bottom <= otherBottom) ? bottom : otherBottom;
            int intersectionHeight = intersectionBottom - intersectionTop;

            Left = intersectionLeft;
            Top = intersectionTop;
            Width = intersectionWidth;
            Height = intersectionHeight;
        }

        /// <returns>A string representation of this region.</returns>
        public override string ToString()
        {
            return $"({Left}, {Top}) {Width}x{Height}";
        }

        public Region ExpandToContain(IMutableRegion region)
        {
            int left = Math.Min(this.Left, region.Left);
            int top = Math.Min(this.Top, region.Top);

            int thisRight = this.Left + this.Width;
            int otherRight = region.Left + region.Width;
            int maxRight = Math.Max(thisRight, otherRight);
            int width = maxRight - left;

            int thisBottom = this.Top + this.Height;
            int otherBottom = region.Top + region.Height;
            int maxBottom = Math.Max(thisBottom, otherBottom);
            int height = maxBottom - top;

            return new Region(left, top, width, height);
        }

        public static implicit operator Region(Rectangle rect)
        {
            return new Region(rect);
        }
    }
}
