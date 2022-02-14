namespace Applitools.Utils
{
    public class ScaleProviderIdentityFactory : ScaleProviderFactory
    {
        public ScaleProviderIdentityFactory(IScaleProvider scaleProvider, SetScaleProviderHandler setScaleProvider) 
            : base(scaleProvider, setScaleProvider)
        {
        }
    }
}
