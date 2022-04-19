namespace Applitools
{
    using System;
    using System.Drawing;
    using Applitools.Utils;

    /// <summary>
    /// <see cref="InWindow"/> API.
    /// </summary>
    public class InWindow
    {
        #region Fields

        protected readonly CreateImageHandler CreateImage;
        private string id_;
        
        #endregion

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="InWindow"/> instance tied to the input image id.
        /// </summary>
        public InWindow(string id, CreateImageHandler createImage)
        {
            ArgumentGuard.NotNull(createImage, nameof(createImage));

            id_ = id;
            CreateImage = createImage;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets this image's id
        /// </summary>
        public string Id
        {
            get
            {
                if (id_ == null)
                {
                    id_ = CreateImage(GetImageBounds());
                }

                return id_;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets the bounds of the image to create.
        /// </summary>
        protected virtual Rectangle GetImageBounds()
        {
            return Rectangle.Empty;
        }

        #endregion
    }
}
