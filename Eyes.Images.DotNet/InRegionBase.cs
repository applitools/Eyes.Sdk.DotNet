namespace Applitools
{
    using System.Drawing;
    using Utils;

    /// <summary>
    /// An <see cref="InRegionBase"/> API.
    /// </summary>
    public class InRegionBase : InWindow
    {
        #region Fields

        private readonly GetTextHandler getText_;

        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InRegionBase"/> instance.
        /// </summary>
        /// <param name="id">Id of the region's image</param>
        /// <param name="bounds">The bounds of the region within the application's window</param>
        /// <param name="createImage">Creates a window image and returns its id.</param>
        /// <param name="getText">Returns the text located in an image region</param>
        public InRegionBase(
            string id,
            Rectangle bounds, 
            CreateImageHandler createImage, 
            GetTextHandler getText) 
            : base(id, createImage)
        {
            ArgumentGuard.NotNull(createImage, nameof(createImage));
            ArgumentGuard.NotNull(getText, nameof(getText));

            Bounds = bounds;
            getText_ = getText;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the bounds of this window region.
        /// </summary>
        public Rectangle Bounds { get; private set; }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the text found in this region.
        /// </summary>
        public string GetText()
        {
            // Get the text from the entire region image.
            return getText_(Id, new[] { Rectangle.Empty }, null)[0];
        }

        /// <summary>
        /// Add another window region.
        /// </summary>
        public InRegionsBase And(Rectangle bounds)
        {
            return new InRegionsBase(CreateImage, getText_).And(Bounds).And(bounds);
        }

        /// <inheritdoc />
        protected override Rectangle GetImageBounds()
        {
            return Bounds;
        }

        #endregion
    }
}
