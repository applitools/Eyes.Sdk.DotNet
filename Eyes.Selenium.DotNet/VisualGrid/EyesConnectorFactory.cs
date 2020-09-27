using Applitools.VisualGrid;

namespace Applitools.Selenium.VisualGrid
{
    class EyesConnectorFactory : IEyesConnectorFactory
    {
        public Ufg.IUfgConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            return new EyesConnector(browserInfo, config);
        }
    }
}
