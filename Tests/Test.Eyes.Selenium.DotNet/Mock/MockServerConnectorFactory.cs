using System;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnectorFactory : IServerConnectorFactory
    {
        public IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null)
        {
            return new MockServerConnector(logger, serverUrl);
        }
    }
}
