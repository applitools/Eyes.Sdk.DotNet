using System;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnectorFactory : IServerConnectorFactory
    {
        public IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null)
        {
            logger.Verbose($"creating {nameof(MockServerConnector)}");
            return new MockServerConnector(logger, serverUrl);
        }
    }
}
