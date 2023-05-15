using Applitools.Commands;
using Applitools.Universal;

namespace Applitools
{
    public class CommandExecutor
    {
        private WebSocketClient webSocket_;
        private static CommandExecutor instance_;

        private Logger Logger { get; } = new Logger();

        public static CommandExecutor GetInstance(ISpecDriverMessageAdaptor specDriverMessageAdaptor,
            ILogHandler logHandler = null, string initMessage = null)
        {
            return instance_ ?? (instance_ = new CommandExecutor(specDriverMessageAdaptor, logHandler, initMessage));
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

        private CommandExecutor(ISpecDriverMessageAdaptor specDriverMessageAdaptor, ILogHandler logHandler = null, string initMessage = null)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            StartServer(specDriverMessageAdaptor);

            if (string.IsNullOrEmpty(initMessage) == false)
            {
                webSocket_.SendData(initMessage);
            }
        }

        private void StartServer(ISpecDriverMessageAdaptor specDriverMessageAdaptor)
        {
            int serverPort = UniversalSdkNativeLoader.Start();

            webSocket_ = new WebSocketClient(specDriverMessageAdaptor, Logger.GetILogHandler());

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