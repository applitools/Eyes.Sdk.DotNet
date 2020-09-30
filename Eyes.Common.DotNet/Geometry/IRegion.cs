using System.Drawing;
using Region = Applitools.Utils.Geometry.Region;

namespace Applitools.Utils.Geometry
{
    /// <summary>
    /// A geometrical region.
    /// </summary>
    public interface IRegion
    {
        /// <summary>
        /// X coordinate of this region's left edge.
        /// </summary>
        int Left { get; }

        /// <summary>
        /// X coordinate of this region's right edge.
        /// </summary>
        int Right { get; }

        /// <summary>
        /// Y coordinate of this region's top edge.
        /// </summary>
        int Top { get; }

        /// <summary>
        ///  Y coordinate of this region's bottom edge.
        /// </summary>
        int Bottom { get; }

        /// <summary>
        /// Width of this region in pixels.
        /// </summary>
        int Width { get; }

        /// <summary>
        /// Height of this region in pixels.
        /// </summary>
        int Height { get; }

        /// <summary>
        /// Gets the top-left corner of this region.
        /// </summary>
        Point Location { get; }

        /// <summary>
        /// Gets the size of this region.
        /// </summary>
        Size Size { get; }

        /// <summary>
        /// Gets the area of this region.
        /// </summary>
        int Area { get; }

        /// <summary>
        /// Gets the rectangle of the same position and size as this region.
        /// </summary>
        Rectangle Rectangle { get; }

        /// <summary>
        /// Offsets a clone of this region instance by given amount of pixels.
        /// </summary>
        /// <param name="x">Amount of pixels to move the region horizontally.</param>
        /// <param name="y">Amount of pixels to move the region vertically.</param>
        /// <returns>A clone of this region, offseted by the given amount of pixels.</returns>
        Region Offset(int x, int y);
    }
}
