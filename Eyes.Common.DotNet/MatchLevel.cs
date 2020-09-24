namespace Applitools
{
    /// <summary>
    /// The extent in which two images match (or are expected to match).
    /// </summary>
    public enum MatchLevel
    {
        /// <summary>
        /// Images do not necessarily match.
        /// </summary>
        None,

        /// <summary>
        /// Images have the same layout (comapred using the latest layout comparison engine).
        /// </summary>
        Layout,

        /// <summary>
        /// Images have the same layout (compared using layout comparison engine v1).
        /// </summary>
        Layout1,

        /// <summary>
        /// Images have the same layout (compared using layout comparison engine v2).
        /// </summary>
        Layout2,

        /// <summary>
        /// Images have the same content.
        /// </summary>
        Content,

        /// <summary>
        /// Images are nearly identical.
        /// </summary>
        Strict,

        /// <summary>
        /// Images are identical.
        /// </summary>
        Exact
    }
}
