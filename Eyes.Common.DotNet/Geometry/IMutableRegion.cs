namespace Applitools.Utils.Geometry
{
    /// <summary>
    /// A mutable geometrical region.
    /// </summary>
    public interface IMutableRegion : IRegion
    {
        /// <summary>
        /// Gets or sets the X coordinate of this region's left edge.
        /// </summary>
        new int Left { get; set; }

        /// <summary>
        /// Gets or sets the Y coordinate of this region's top edge.
        /// </summary>
        new int Top { get; set; }

        /// <summary>
        /// Gets or sets the width of this region in pixels.
        /// </summary>
        new int Width { get; set; }

        /// <summary>
        /// Gets or sets the height of this region in pixels.
        /// </summary>
        new int Height { get; set; }

        /// <summary>
        /// Offsets this instance by a given amount of pixels.
        /// </summary>
        /// <param name="x">Amount of pixels to move the region horizontally.</param>
        /// <param name="y">Amount of pixels to move the region vertically.</param>
        new void Offset(int x, int y);
    }
}
