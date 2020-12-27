namespace Applitools.VisualGrid
{
    public interface IEyesConnectorFactory
    {
        IEyesBase CreateNewEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Configuration config);
    }
}
