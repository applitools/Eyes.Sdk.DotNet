using Applitools.VisualGrid;

namespace Applitools.Selenium.Tests.Mock
{
    class MockEyesConnectorFactory : IEyesConnectorFactory
    {
        public Ufg.IUfgConnector CreateNewEyesConnector(Logger logger, RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            return new MockEyesConnector(logger, browserInfo, config);
        }
    }
}
