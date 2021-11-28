namespace Applitools.Selenium
{
    using System.Drawing;
    using Utils;
    using Utils.Images;

    /// <summary>
    /// Scale provider which determines the scale ratio according to the context.
    /// </summary>
    class ContextBasedScaleProvider : IScaleProvider
    {
        private static readonly int ALLOWED_VIEWPORT_SIZE_DEVIATION_ = 1;
        private static readonly int ALLOWED_DCES_DEVIATION_ = 10;
        private static readonly int UNKNOWN_SCALE_RATIO_ = 0;

        #region Constructors

        /// <summary>
        /// creates a new <see cref="ContextBasedScaleProvider" /> instance.
        /// </summary>
        /// <param name="topLevelContextSize">The total size of the top level context. E.g., for 
        /// Selenium this would be the document size of the top level frame.</param>
        /// <param name="viewportSize">The viewport size.</param>
        /// <param name="devicePixelRatio">The device pixel ratio of the platform of which the 
        /// application is running.</param>
        public ContextBasedScaleProvider(Size topLevelContextSize, Size viewportSize,
            double devicePixelRatio)
        {
            ArgumentGuard.GreaterThan(devicePixelRatio, 0, nameof(devicePixelRatio));

            ScaleRatio_ = UNKNOWN_SCALE_RATIO_;
            TopLevelContextSize = topLevelContextSize;
            ViewportSize = viewportSize;
            DevicePixelRatio = devicePixelRatio;
        }

        #endregion

        #region Properties

        public Size TopLevelContextSize { get; private set; }
        public Size ViewportSize { get; private set; }
        public double DevicePixelRatio { get; private set; }
        private double ScaleRatio_ { get; set; }

        /// <inheritdoc />
        public double ScaleRatio
        {
            get
            {
                ArgumentGuard.IsValidState(ScaleRatio_ != UNKNOWN_SCALE_RATIO_, "scale ratio not defined yet");
                return ScaleRatio_;
            }
        }
        #endregion 

        #region Methods

        ///// <inheritdoc />
        //public Bitmap ScaleImage(Bitmap image)
        //{
        //    ArgumentGuard.NotNull(image, nameof(image));
        //    if (ScaleRatio_ == UNKNOWN_SCALE_RATIO_)
        //    {
        //        var imageWidth = image.Width;
        //        UpdateScaleRatio(imageWidth);
        //    }

        //    Image scaledImage = BasicImageUtils.ScaleImage(image, ScaleRatio_);
        //    return (Bitmap)scaledImage;
        //}

        public void UpdateScaleRatio(int imageWidth)
        {
            var viewportWidth = ViewportSize.Width;
            var dcesWidth = TopLevelContextSize.Width;

            // If the image's width is the same as the viewport's width or the
            // top level context's width, no scaling is necessary.
            if (((imageWidth >= viewportWidth - ALLOWED_VIEWPORT_SIZE_DEVIATION_)
                        && (imageWidth <= viewportWidth + ALLOWED_VIEWPORT_SIZE_DEVIATION_))
                    || ((imageWidth >= dcesWidth - ALLOWED_DCES_DEVIATION_)
                        && imageWidth <= dcesWidth + ALLOWED_DCES_DEVIATION_))
            {
                ScaleRatio_ = 1;
            }
            else
            {
                ScaleRatio_ = 1 / DevicePixelRatio;
            }
        }

        public override string ToString()
        {
            return $"{nameof(ContextBasedScaleProvider)} (TopLevelContextSize: {TopLevelContextSize} ; ViewportSize: {ViewportSize} ; DevicePixelRatio: {DevicePixelRatio} ; ScaleRatio_: {ScaleRatio_})";
        }

        #endregion
    }
}
