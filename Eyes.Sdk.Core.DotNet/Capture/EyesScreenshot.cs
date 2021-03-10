using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;
using Applitools.Utils;
using Applitools.Utils.Geometry;

namespace Applitools
{
    public abstract class EyesScreenshot : IEyesScreenshot
    {
        public EyesScreenshot(Bitmap image)
        {
            ArgumentGuard.NotNull(image, nameof(image));
            Image = image;
        }

        public Bitmap Image { get; private set; }

        public string DomUrl { get; set; }

        public Point OriginLocation { get; set; }

        /// <summary>
        /// Returns a part of the screenshot based on the given region.
        /// </summary>
        /// <param name="region">The region for which we should get the sub screenshot.</param>
        /// <param name="throwIfClipped">Throw an EyesException if the region is not fully contained in the screenshot.</param>
        /// 
        /// <returns>A screenshot instance containing the given region.</returns>
        public abstract EyesScreenshot GetSubScreenshot(Region region, bool throwIfClipped);

        /// <summary>
        /// Converts a location's coordinates with the <paramref name="from"/> coordinates type
        /// to the <paramref name="to"/> coordinates type.
        /// </summary>
        /// <param name="location">The location which coordinates needs to be converted.</param>
        /// <param name="from">The current coordinates type for <paramref name="location"/></param>
        /// <param name="to">The target coordinates type for <paramref name="location"/></param>
        /// <returns>A new location which is the transformation of <paramref name="location"/> to the <paramref name="to"/> coordinates type.</returns>
        public abstract Point ConvertLocation(Point location, CoordinatesTypeEnum from, CoordinatesTypeEnum to);

        /// <summary>
        /// Calculates the location in the screenshot of the location given as parameter.
        /// </summary>
        /// <param name="location">The location as coordinates inside the current frame.</param>
        /// <param name="coordinatesType">The coordinates type of <paramref name="location"/></param>
        /// <returns>The corresponding location inside the screenshot, in screenshot as-is coordinates type.</returns>
        /// <exception cref="OutOfBoundsException">If the location is not inside the frame's region in the screenshot.</exception>
        public abstract Point GetLocationInScreenshot(Point location, CoordinatesTypeEnum coordinatesType);

        /// <summary>
        /// Get the intersection of the given region with the screenshot.
        /// </summary>
        /// <param name="region">region The region to intersect.</param>
        /// <param name="originalCoordinatesType">The coordinates type of <paramref name="region"/>.</param>
        /// <param name="resultCoordinatesType">The coordinates type of the resulting region.</param>
        /// <returns>The intersected region, in <paramref name="resultCoordinatesType"/> coordinates.</returns>
        public abstract Region GetIntersectedRegion(Region region, CoordinatesTypeEnum originalCoordinatesType, CoordinatesTypeEnum resultCoordinatesType);

        /// <summary>
        /// Get the intersection of the given region with the screenshot.
        /// </summary>
        /// <param name="region">region The region to intersect.</param>
        /// <param name="coordinatesType">The coordinates type of <paramref name="region"/>.</param>
        /// <returns>The intersected region, in <paramref name="coordinatesType"/> coordinates.</returns>
        public Region GetIntersectedRegion(Region region, CoordinatesTypeEnum coordinatesType)
        {
            return GetIntersectedRegion(region, region.CoordinatesType, coordinatesType);
        }

        /// <summary>
        /// Converts a region's location coordinates with the <paramref name="from"/> coordinates type to the <paramref name="to"/> coordinates type.
        /// </summary>
        /// <param name="region">The region which location's coordinates needs to be converted.</param>
        /// <param name="from">The current coordinates type for <paramref name="region"/>.</param>
        /// <param name="to"> The target coordinates type for <paramref name="region"/>.</param>
        /// <returns>A new region which is the transformation of <paramref name="region"/> to the <paramref name="to"/> coordinates type.</returns>
        public Region ConvertRegionLocation(Region region, CoordinatesTypeEnum from, CoordinatesTypeEnum to)
        {
            ArgumentGuard.NotNull(region, nameof(region));

            if (region.IsSizeEmpty)
            {
                return region;
            }

            ArgumentGuard.NotNull(from, nameof(from));
            ArgumentGuard.NotNull(to, nameof(to));

            Point updatedLocation = ConvertLocation(region.Location, from, to);

            return new Region(updatedLocation, region.Size);
        }

        public override string ToString()
        {
            return $"{GetType()} - {Image.Width}x{Image.Height}";
        }

        internal void DisposeImage()
        {
            Image?.Dispose();
            Image = null;
        }
    }
}
