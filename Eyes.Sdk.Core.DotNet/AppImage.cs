namespace Applitools
{
    /// <summary>
    /// An image taken from the application under test and matched against the baseline.
    /// </summary>
    public class AppImage
    {
        /// <summary>
        /// Creates a new <see cref="AppImage"/> instance.
        /// </summary>
        public AppImage(bool isMatch)
        {
            IsMatch = isMatch;
        }

        /// <summary>
        /// Whether or not the image matched the expected baseline image.
        /// </summary>
        public bool IsMatch { get; private set; }
    }
}
