namespace Applitools.VisualGrid
{
    public interface IEyesConnectorFactory
    {
        IEyesConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Configuration config);
    }
}
