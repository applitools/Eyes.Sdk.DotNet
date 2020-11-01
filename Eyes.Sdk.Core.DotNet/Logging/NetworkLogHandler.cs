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

        public override void OnMessage(string message, TraceLevel level = default)
        {
            lock (clientEvents_)
            {
                string currentTime = TimeUtils.ToString(DateTimeOffset.Now, StandardDateTimeFormat.ISO8601);
                ClientEvent clientEvent = new ClientEvent(currentTime, message, level);
                clientEvents_.Add(clientEvent);
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

        public static void SendSingleLog(IServerConnector serverConnector, TraceLevel level, string message, params object[] args)
        {
            NetworkLogHandler logHandler = new NetworkLogHandler(serverConnector);
            logHandler.OnMessage(level, message, args);
            logHandler.Close();
        }
    }
}
