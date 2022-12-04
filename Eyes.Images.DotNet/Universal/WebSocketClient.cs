using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools
{
    public delegate void SocketMessageReceivedHandler(string message);

    public class WebSocketClient //: IUniversalSocketClient
    {
        private readonly ClientWebSocket clientWebSocket_;
        private CancellationTokenSource cancellationTokenSource_;

        public WebSocketClient()
        {
            clientWebSocket_ = new ClientWebSocket();
        }

        public event SocketMessageReceivedHandler MessageReceived;

        public Task ConnectAsync(string url)
        {
            cancellationTokenSource_ = new CancellationTokenSource();
            return clientWebSocket_.ConnectAsync(new Uri(url), cancellationTokenSource_.Token);
        }

        public Task SendData(string message)
        {
            return SendStringAsync(message, CancellationToken.None);
        }

        public Task SendData(string message, CancellationToken cancellationToken)
        {
            return SendStringAsync(message, cancellationToken);
        }

        private object _lockObject = new object();
        private AutoResetEvent _event = new AutoResetEvent(true);

        public async Task<string> ReceiveMessage()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result;
            var allBytes = new List<byte>();
            _event.WaitOne();
            do
            {
                result = await clientWebSocket_.ReceiveAsync(buffer, CancellationToken.None).ConfigureAwait(false);
                for (int i = 0; i < result.Count; i++)
                {
                    allBytes.Add(buffer.Array[i]);
                }
            } while (!result.EndOfMessage);

            var text = Encoding.UTF8.GetString(allBytes.ToArray(), 0, allBytes.Count);
            _event.Set();
            return text;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            cancellationTokenSource_.Dispose();
            clientWebSocket_.Dispose();
        }

        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
        }

        private async Task SendStringAsync(string data, CancellationToken cancellationToken)
        {
            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            await clientWebSocket_.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken).ConfigureAwait(false);
        }
    }
}