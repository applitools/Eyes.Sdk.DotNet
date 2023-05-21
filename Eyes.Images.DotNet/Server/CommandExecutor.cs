using Applitools.Commands;
using Applitools.Universal;

namespace Applitools
{
    public class CommandExecutor
    {
        private WebSocketClient webSocket_;
        private static CommandExecutor instance_;

        private Logger Logger { get; } = new Logger();

        public static CommandExecutor GetInstance(ISpecDriverMessageListener specDriverMessageListener,
            ILogHandler logHandler = null, string initMessage = null)
        {
            return instance_ ?? (instance_ = new CommandExecutor(specDriverMessageListener, logHandler, initMessage));
        }

        public void SendData<TRequest>(TRequest data) where TRequest : CommandBase
        {
            webSocket_.SendData(data);
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest data) 
            where TRequest : CommandBase 
            where TResponse : CommandBase
        {
            return webSocket_.SendRequest<TRequest, TResponse>(data);
        }

        public void SetLogHandler(ILogHandler logHandler)
        {
            webSocket_.SetLogHandler(logHandler);
        }

        private CommandExecutor(ISpecDriverMessageListener specDriverMessageListener, ILogHandler logHandler = null, string initMessage = null)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            StartServer(specDriverMessageListener);

            if (string.IsNullOrEmpty(initMessage) == false)
            {
                webSocket_.SendData(initMessage);
            }
            //webSocket_.MessageReceived+=
        }

        private void StartServer(ISpecDriverMessageListener specDriverMessageListener)
        {
            int serverPort = UniversalSdkNativeLoader.Start();

            webSocket_ = new WebSocketClient(specDriverMessageListener, Logger.GetILogHandler());
            specDriverMessageListener.WebSocket = webSocket_;
            var url = GetSocketUrl_(serverPort);
            Logger.Log($"Server url: {url}");

            webSocket_.ConnectAsync(url);

            Logger.Log("Connected to Server");
        }

        private static string GetSocketUrl_(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }
    }
}