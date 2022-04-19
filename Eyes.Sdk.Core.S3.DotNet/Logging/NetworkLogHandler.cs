using Applitools.Utils;
using System;

namespace Applitools
{
    public class NetworkLogHandler : LogHandlerBase
    {
        private static readonly int MAX_EVENTS_SIZE = 100;

        private readonly IServerConnector serverConnector_;
        private readonly LogSessionsClientEvents clientEvents_;

        protected internal NetworkLogHandler(IServerConnector serverConnector)
            : base(true)
        {
            ArgumentGuard.NotNull(serverConnector, nameof(serverConnector));
            isOpen_ = false;
            serverConnector_ = serverConnector;
            clientEvents_ = new LogSessionsClientEvents();
        }

        public void OnEvent(object @event, TraceLevel level = default)
        {
            string currentTime = DateTimeOffset.UtcNow.ToString(StandardDateTimeFormat.ISO8601);
            ClientEvent clientEvent = new ClientEvent(currentTime, @event, level);
            OnMessage(clientEvent);
        }

        public override void OnMessage(string message, TraceLevel level = default)
        {
            OnEvent(message, level);
        }

        public override void OnMessage(ClientEvent @event)
        {
            lock (clientEvents_)
            {
                clientEvents_.Add(@event);
                if (clientEvents_.Count >= MAX_EVENTS_SIZE)
                {
                    SendLogs_();
                }
            }
        }

        public override void Close()
        {
            SendLogs_();
        }

        private void SendLogs_()
        {
            lock (clientEvents_)
            {
                if (clientEvents_.Count == 0)
                {
                    return;
                }

                serverConnector_.SendLogs(clientEvents_);

                clientEvents_.Clear();
            }
        }

        public static void SendEvent(IServerConnector serverConnector, TraceLevel level, object @event)
        {
            NetworkLogHandler logHandler = new NetworkLogHandler(serverConnector);
            logHandler.OnEvent(@event, level);
            logHandler.Close();
        }
    }
}
