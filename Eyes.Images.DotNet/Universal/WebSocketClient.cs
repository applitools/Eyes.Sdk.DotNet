using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Applitools.Commands;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools
{
    internal class ResponseMessage
    {
        public string Key { get; set; }
    }

    public delegate void SocketMessageReceivedHandler(string message);

    public class WebSocketClient //: IUniversalSocketClient
    {
        private readonly ClientWebSocket webSocket_;
        private CancellationTokenSource cancellationTokenSource_;
        private readonly ConcurrentDictionary<string, string> messages_;
        private readonly object lock_ = new object();

        public event SocketMessageReceivedHandler MessageReceived;

        private Logger Logger { get; } = new Logger();
        
        public WebSocketClient(ILogHandler logHandler = null)
        {
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }
            webSocket_ = new ClientWebSocket();
            messages_ = new ConcurrentDictionary<string, string>();
        }

        public void ConnectAsync(string url)
        {
            lock (lock_)
            {
                if (webSocket_.State == WebSocketState.Open)
                {
                    return;
                }

                cancellationTokenSource_ = new CancellationTokenSource();
                webSocket_.ConnectAsync(new Uri(url), cancellationTokenSource_.Token).ConfigureAwait(false).GetAwaiter().GetResult();

                while (webSocket_.State == WebSocketState.Connecting)
                {
                    Thread.Sleep(10);
                }
                
                if (webSocket_.State != WebSocketState.Open)
                {
                    throw new Exception("Connection is not opened.");
                }
                Task.Factory.StartNew(ReadMessage, TaskCreationOptions.LongRunning);
            }
        }

        public TResponse SendRequest<TRequest, TResponse>(TRequest data)
            where TRequest : CommandBase
            where TResponse : CommandBase
        {
            var json = data.ToJson();

            SendData(json);

            var message = WaitForMessage(data.Key);
            if (message == null)
            {
                throw new Exception($"Server did not respond for {data.Name}");
            }

            var result = JsonConvert.DeserializeObject<TResponse>(message);
            return result;
        }

        public void SendData<TRequest>(TRequest data)
            where TRequest : CommandBase
        {
            var json = data.ToJson();

            SendData(json);
        }

        public void SendData(string message)
        {
            SendString(message, CancellationToken.None);
        }

        public string ReceiveMessage(string key)
        {
            if (messages_.TryRemove(key, out string value))
            {
                return value;
            }

            return null;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            cancellationTokenSource_.Dispose();
            webSocket_.Dispose();
        }

        public void SetLogHandler(ILogHandler logHandler)
        {
            Logger.SetLogHandler(logHandler);
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
        }

        private string WaitForMessage(string key)
        {
            // timeout 9 minutes
            for (int i = 0; i < 2700; i++)
            {
                var message = ReceiveMessage(key);
                if (message != null)
                {
                    return message;
                }
                Thread.Sleep(200);
            }

            return null;
        }

        private void ReadMessage()
        {
            while (true)
            {
                var textMessage = ReceiveMessage();

                var response = JsonConvert.DeserializeObject<ResponseMessage>(textMessage);
                if (response.Key != null)
                {
                    if (messages_.TryAdd(response.Key, textMessage) == false)
                    {
                        throw new Exception($"Cannot add key {response.Key} with message {textMessage}");
                    }
                }

                Thread.Sleep(100);
            }
        }

        private string ReceiveMessage()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result;
            var allBytes = new List<byte>();

            do
            {
                result = webSocket_.ReceiveAsync(buffer, CancellationToken.None).GetAwaiter().GetResult();
                for (int i = 0; i < result.Count; i++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    allBytes.Add(buffer.Array[i]);
                }
            } while (!result.EndOfMessage);

            var text = Encoding.UTF8.GetString(allBytes.ToArray(), 0, allBytes.Count);

            Logger.Log($"In message: {Environment.NewLine} {text}");
            return text;
        }

        private void SendString(string data, CancellationToken cancellationToken)
        {
            lock (lock_)
            {
                byte[] encoded = Encoding.UTF8.GetBytes(data);
                var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
                webSocket_.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken).GetAwaiter().GetResult();

                Logger.Log($"Request sent to Server: {Environment.NewLine} {data}");
            }
        }
    }
}