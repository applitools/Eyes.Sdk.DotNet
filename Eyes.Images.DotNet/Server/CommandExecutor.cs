using System;
using System.Diagnostics;
using Applitools.Commands;
using Applitools.Commands.Requests;
using Applitools.Universal;

namespace Applitools
{
    public class CommandExecutor
    {
        private WebSocketClient webSocket_;
        private static CommandExecutor instance_;
        private static readonly object locker_ = new object();

        private Logger Logger { get; } = new Logger();

        public static CommandExecutor GetInstance(CoreMakeSdkRequest coreSdkRequest,
            ISpecDriverMessageListener specDriverMessageListener,
            ILogHandler logHandler = null, string initMessage = null)
        {
            if (instance_ != null)
            {
                return instance_;
            }

            lock (locker_)
            {
                instance_ = instance_ ??
                            new CommandExecutor(coreSdkRequest, specDriverMessageListener, logHandler, initMessage);
            }

            return instance_;
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

        private CommandExecutor(CoreMakeSdkRequest coreSdkRequest,
            ISpecDriverMessageListener specDriverMessageListener,
            ILogHandler logHandler = null, string initMessage = null)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            StartServer(specDriverMessageListener);
            webSocket_.SendData(coreSdkRequest);

            if (string.IsNullOrEmpty(initMessage) == false)
            {
                webSocket_.SendData(initMessage);
            }
        }

        private void MakeCoreSdk_(string agentId, string spec)
        {
            var result = new CoreMakeSdkRequest
            {
                Payload = new MakeCorePayload
                {
                    AgentId = $"{agentId}",
                    Cwd = Environment.CurrentDirectory,
                    Protocol = spec
                }
            };
        }

        private void StartServer(ISpecDriverMessageListener specDriverMessageListener)
        {
            int serverPort = UniversalSdkNativeLoader.Start();

            webSocket_ = new WebSocketClient(specDriverMessageListener, Logger.GetILogHandler());
            specDriverMessageListener.WebSocket = webSocket_;
            var url = GetSocketUrl_(serverPort);
            Logger.Log($"Server url: {url} ; current process id: {Process.GetCurrentProcess().Id}");

            webSocket_.ConnectAsync(url);

            Logger.Log("Connected to Server");
        }

        private static string GetSocketUrl_(int port)
        {
            return $"ws://localhost:{port}/eyes";
        }
    }
}