using System;

namespace Applitools
{
    class ServerConnectorFactory : IServerConnectorFactory
    {
        public IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null)
        {
            return new ServerConnector(logger, serverUrl);
        }
    }
}
