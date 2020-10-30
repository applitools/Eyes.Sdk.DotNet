using Applitools.VisualGrid;

namespace Applitools.Selenium.VisualGrid
{
    class EyesConnectorFactory : IEyesConnectorFactory
    {
        public Ufg.IUfgConnector CreateNewEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            logger.Verbose($"creating {nameof(EyesConnector)}");
            return new EyesConnector(logger, browserInfo, config);
        }
    }
}
