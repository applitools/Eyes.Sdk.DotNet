namespace Applitools.Utils.Geometry
{
    /// <summary>
    /// Encapsulates the type of coordinates used by the region.
    /// </summary>
    public enum CoordinatesTypeEnum
    {
        /// <summary>
        /// The coordinates should be used "as is" on the screenshot image, regardless of the current context.
        /// </summary>
        SCREENSHOT_AS_IS,

        /// <summary>
        /// The coordinates should be used "as is" within the current context. For
        /// example, if we're inside a frame, the coordinates are "as is", but within the current frame's viewport.
        /// </summary>
        CONTEXT_AS_IS,

        ///<summary>
        /// Coordinates are relative to the context. For example, if we are in a context of a frame in a web page,
        /// then the coordinates are relative to the frame. In this case, if we want to crop an image region based on
        /// an element's region, we will need to calculate their respective "as is" coordinates.
        ///</summary>
        CONTEXT_RELATIVE
    }
}