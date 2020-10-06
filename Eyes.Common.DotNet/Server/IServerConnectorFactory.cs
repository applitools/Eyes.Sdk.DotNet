using System;

namespace Applitools
{
    public interface IServerConnectorFactory
    {
        IServerConnector CreateNewServerConnector(Logger logger, Uri serverUrl = null);
    }
}
