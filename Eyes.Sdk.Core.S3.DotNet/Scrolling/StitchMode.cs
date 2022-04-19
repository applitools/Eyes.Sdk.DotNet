namespace Applitools
{
    /// <summary>
    /// The type of stitching to use (e.g., when doing a full page screenshot).
    /// </summary>
    public enum StitchModes
    {
        /// <summary>
        /// Standard JS scrolling.
        /// </summary>
        Scroll = 0,

        /// <summary>
        /// CSS translation based stitching.
        /// </summary>
        CSS = 10
    }
}
