namespace Applitools.VisualGrid
{
    public interface IEyesConnectorFactory
    {
        Ufg.IUfgConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Configuration config);
    }
}
