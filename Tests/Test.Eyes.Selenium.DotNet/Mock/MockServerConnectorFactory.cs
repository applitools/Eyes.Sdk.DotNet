using OpenQA.Selenium;
using System;

namespace Applitools.Selenium.Tests.Mock
{
    class MockServerConnectorFactory : IServerConnectorFactory
    {
        private WebDriverProvider driverProvider_;

        public MockServerConnectorFactory(WebDriverProvider driverProvider)
        {
            driverProvider_ = driverProvider;
        }

        public IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null)
        {
            logger.Verbose($"creating {nameof(MockServerConnector)}");
            return new MockServerConnector(logger, serverUrl, driverProvider_);
        }
    }
}
