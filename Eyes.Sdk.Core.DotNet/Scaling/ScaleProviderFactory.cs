namespace Applitools.Utils
{

    /// <summary>
    /// Sets the scale provider used for scaling images before validation.
    /// </summary>
    /// <param name="scaleProvider">The scale provider to use</param>
    public delegate void SetScaleProviderHandler(IScaleProvider scaleProvider);

    /// <summary>
    /// Abstraction for instantiating scale providers.
    /// </summary>
    public abstract class ScaleProviderFactory
    {
        private IScaleProvider scaleProvider_;
        private readonly SetScaleProviderHandler setScaleProviderHandler_;

        public ScaleProviderFactory(IScaleProvider scaleProvider, SetScaleProviderHandler setScaleProvider)
        {
            scaleProvider_ = scaleProvider;
            setScaleProviderHandler_ = setScaleProvider;
        }

        public IScaleProvider GetScaleProvider(int imageToScaleWidth)
        {
            scaleProvider_ = GetScaleProviderImpl(imageToScaleWidth);
            setScaleProviderHandler_(scaleProvider_);
            return scaleProvider_;
        }

        protected virtual IScaleProvider GetScaleProviderImpl(int imageToScaleWidth)
        {
            return scaleProvider_;
        }
    }
}
