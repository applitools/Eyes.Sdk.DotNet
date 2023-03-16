using Applitools.Commands;

namespace Applitools
{
    public class CommandExecutor
    {
        private WebSocketClient _webSocket;
        private static CommandExecutor _instance;
        
        public Logger Logger { get; } = new Logger();

        public static CommandExecutor GetInstance(ILogHandler logHandler = null, string initMessage = null)
        {
            return _instance ?? (_instance = new CommandExecutor(logHandler, initMessage));
        }

        public void SendData<TRequest>(TRequest data) where TRequest : CommandBase
        {
            _webSocket.SendData(data);
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest data) 
            where TRequest : CommandBase 
            where TResponse : CommandBase
        {
            return _webSocket.SendRequest<TRequest, TResponse>(data);
        }

        public void SetLogHandler(ILogHandler logHandler)
        {
            _webSocket.SetLogHandler(logHandler);
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
                _webSocket.SendData(initMessage);
            }
        }

        private void StartServer()
        {
            int serverPort = UniversalSdkNativeLoader.Start();

            _webSocket = new WebSocketClient(Logger?.GetILogHandler());

            var url = GetSocketUrl(serverPort);
            Logger.Log($"Server url: {url}");

            _webSocket.ConnectAsync(url);

            Logger.Log("Connected to Server");
        }

        private string GetSocketUrl(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }
    }
}