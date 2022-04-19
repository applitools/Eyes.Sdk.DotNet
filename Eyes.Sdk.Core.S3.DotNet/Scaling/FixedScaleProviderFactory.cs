namespace Applitools.Utils
{
    public class FixedScaleProviderFactory : ScaleProviderFactory
    {
        public FixedScaleProviderFactory(double scaleRatio, SetScaleProviderHandler setScaleProvider)
            : base(new FixedScaleProvider(scaleRatio), setScaleProvider) { }
    }
}
