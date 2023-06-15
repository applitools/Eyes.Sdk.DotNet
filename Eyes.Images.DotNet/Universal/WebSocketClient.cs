using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Applitools.Commands;
using Applitools.Universal;
using Applitools.Utils;
using Newtonsoft.Json;

namespace Applitools
{
    internal class ResponseMessage
    {
        // Setter used by Json Deserializer
        // ReSharper disable once UnusedAutoPropertyAccessor.Global
        public string Key { get; set; }
    }

    public class WebSocketClient : IDisposable
    {
        private readonly ClientWebSocket webSocket_;
        private CancellationTokenSource cancellationTokenSource_;
        private readonly ConcurrentDictionary<string, WsMessage> messages_;
        private readonly ConcurrentDictionary<string, object> requestKeys_ = new ConcurrentDictionary<string, object>();
        private readonly object lock_ = new object();
        private static readonly TimeSpan timeout_ = TimeSpan.FromMinutes(9);
        private static readonly JsonSerializer serializer_ = JsonUtils.CreateSerializer();
        
        private readonly ISpecDriverMessageListener messageListener_;

        private Logger Logger { get; } = new Logger();

        public WebSocketClient(ISpecDriverMessageListener messageListener, ILogHandler logHandler = null)
        {
            ArgumentGuard.NotNull(messageListener, nameof(messageListener));
            messageListener_ = messageListener;

            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            webSocket_ = new ClientWebSocket();
            messages_ = new ConcurrentDictionary<string, WsMessage>();
            messageListener_.SetMessagesDictionary(messages_);
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
                webSocket_.ConnectAsync(new Uri(url), cancellationTokenSource_.Token).ConfigureAwait(false).GetAwaiter()
                    .GetResult();

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
            SendData(data);

            var message = WaitForMessage(data.Key);
            if (message == null)
            {
                throw new Exception($"Server did not respond for {data.Name}");
            }

            var result = serializer_.Deserialize<TResponse>(message);
            return result;
        }

        public void SendData<TRequest>(TRequest data)
            where TRequest : CommandBase
        {
            requestKeys_.TryAdd(data.Key, null);
            var json = data.ToJson();

            SendData(json);
        }

        public void SendData(string message)
        {
            SendString(message, CancellationToken.None);
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

        private string WaitForMessage(string key)
        {
            // timeout 9 minutes
            Stopwatch sw = Stopwatch.StartNew();
            while (sw.Elapsed < timeout_)
            {
                if (messages_.TryGetValue(key, out WsMessage message) && message.IsHandled)
                {
                    messages_.TryRemove(key, out _);
                    requestKeys_.TryRemove(key, out _);
                    return message.Message;
                }

                Thread.Sleep(33);
            }

            return null;
        }

        private async void ReadMessage()
        {
            while (true)
            {
                var textMessage = await ReceiveMessageAsync();

                Logger.Log(TraceLevel.Debug, Stage.SpecDriver, StageType.MessageReceived,
                    new { Message = textMessage });
                var response = serializer_.Deserialize<ResponseMessage>(textMessage);
                if (response.Key != null)
                {
                    if (requestKeys_.ContainsKey(response.Key) &&
                        !messages_.TryAdd(response.Key, new WsMessage(textMessage)))
                    {
                        throw new Exception($"Cannot add key {response.Key} with message {textMessage}");
                    }
                }

                messageListener_.HandleMessage(textMessage);
            }
        }

        private async Task<string> ReceiveMessageAsync()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result;
            var allBytes = new List<byte>();

            do
            {
                result = await webSocket_.ReceiveAsync(buffer, CancellationToken.None);
                for (int i = 0; i < result.Count; i++)
                {
                    // ReSharper disable once PossibleNullReferenceException
                    allBytes.Add(buffer.Array[i]);
                }
            } while (!result.EndOfMessage);

            var text = Encoding.UTF8.GetString(allBytes.ToArray(), 0, allBytes.Count);

            //Logger.Log($"In message: {Environment.NewLine} {text}");
            return text;
        }

        private void SendString(string data, CancellationToken cancellationToken)
        {
            lock (lock_)
            {
                byte[] encoded = Encoding.UTF8.GetBytes(data);
                var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
                webSocket_.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken).GetAwaiter()
                    .GetResult();

                //Logger.Log($"Message sent to Server: {Environment.NewLine} {data}");
            }
        }
    }
}