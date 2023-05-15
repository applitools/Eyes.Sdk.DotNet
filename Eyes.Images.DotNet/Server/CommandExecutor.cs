using Applitools.Commands;

namespace Applitools
{
    public class CommandExecutor
    {
        private WebSocketClient webSocket_;
        private static CommandExecutor instance_;

        private Logger Logger { get; } = new Logger();

        public static CommandExecutor GetInstance(ILogHandler logHandler = null, string initMessage = null)
        {
            return instance_ ?? (instance_ = new CommandExecutor(logHandler, initMessage));
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

        private CommandExecutor(ILogHandler logHandler = null, string initMessage = null)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            StartServer();

            if (string.IsNullOrEmpty(initMessage) == false)
            {
                webSocket_.SendData(initMessage);
            }
        }

        private void StartServer()
        {
            int serverPort = UniversalSdkNativeLoader.Start();

            webSocket_ = new WebSocketClient(Logger.GetILogHandler());

            var url = GetSocketUrl(serverPort);
            Logger.Log($"Server url: {url}");

            webSocket_.ConnectAsync(url);

            Logger.Log("Connected to Server");
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }
    }
}