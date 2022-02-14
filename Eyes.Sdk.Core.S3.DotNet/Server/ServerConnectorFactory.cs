using System;

namespace Applitools
{
    class ServerConnectorFactory : IServerConnectorFactory
    {
        public IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null)
        {
            logger.Verbose($"creating {nameof(ServerConnector)}");
            return new ServerConnector(logger, serverUrl);
        }
    }
}
