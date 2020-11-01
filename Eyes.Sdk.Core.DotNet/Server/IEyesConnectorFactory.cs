namespace Applitools.VisualGrid
{
    public interface IEyesConnectorFactory
    {
        Ufg.IUfgConnector CreateNewEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Configuration config);
    }
}
