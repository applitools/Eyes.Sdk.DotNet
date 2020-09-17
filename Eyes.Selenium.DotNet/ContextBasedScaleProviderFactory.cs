namespace Applitools.Selenium
{
    using System.Drawing;
    using Utils;

    /// <summary>
    /// Factory implementation for creating <see cref="ContextBasedScaleProvider"/> instances.
    /// </summary>
    class ContextBasedScaleProviderFactory : ScaleProviderFactory
    {
        private readonly Size TopLevelContextEntireSize_;
        private readonly Size ViewportSize_;
        private readonly double DevicePixelRatio_;

        public ContextBasedScaleProviderFactory(Size topLevelContextEntireSize,
                                                Size viewportSize,
                                                double devicePixelRatio,
                                                IScaleProvider scaleProvider,
                                                SetScaleProviderHandler setScaleProvider)
            : base(scaleProvider, setScaleProvider)
        {
            TopLevelContextEntireSize_ = topLevelContextEntireSize;
            ViewportSize_ = viewportSize;
            DevicePixelRatio_ = devicePixelRatio;
        }

        protected override IScaleProvider GetScaleProviderImpl(int imageToScaleWidth)
        {
            ContextBasedScaleProvider scaleProvider = new ContextBasedScaleProvider(TopLevelContextEntireSize_, ViewportSize_, DevicePixelRatio_);
            scaleProvider.UpdateScaleRatio(imageToScaleWidth);
            return scaleProvider;
        }
    }
}
