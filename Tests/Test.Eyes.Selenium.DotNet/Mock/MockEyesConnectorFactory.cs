using Applitools.VisualGrid;

namespace Applitools.Selenium.Tests.Mock
{
    class MockEyesConnectorFactory : IEyesConnectorFactory
    {
        public Ufg.IEyesConnector CreateNewEyesConnector(RenderBrowserInfo browserInfo, Applitools.Configuration config)
        {
            return new MockEyesConnector(browserInfo, config);
        }
    }
}
