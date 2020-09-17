using Applitools.VisualGrid;

namespace Applitools.Selenium.VisualGrid
{
    class EyesConnectorFactory : IEyesConnectorFactory
    {
        public IEyesConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            return new EyesConnector(browserInfo, config);
        }
    }
}
