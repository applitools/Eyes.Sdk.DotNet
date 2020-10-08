namespace Applitools.VisualGrid
{
    public interface IEyesConnectorFactory
    {
        Ufg.IEyesConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Configuration config);
    }
}
