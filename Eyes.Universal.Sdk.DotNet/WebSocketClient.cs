using System;
using System.Collections.Generic;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Eyes.Universal.Sdk.DotNet
{
    public delegate void SocketMessageReceivedHandler(string message);

    public class WebSocketClient //: IUniversalSocketClient
    {
        public ClientWebSocket clientWebSocket_;
        private CancellationTokenSource cancellationTokenSource;

        public event SocketMessageReceivedHandler MessageReceived;

        public WebSocketClient()
        {
            clientWebSocket_ = new ClientWebSocket();
        }

        public Task ConnectAsync(string url)
        {
            cancellationTokenSource = new CancellationTokenSource();
            return clientWebSocket_.ConnectAsync(new Uri(url), cancellationTokenSource.Token);
        }
        
        public Task SendData(string message)
        {
             return SendStringAsync(message, cancellationTokenSource.Token);
        }

        private async Task SendStringAsync(string data, CancellationToken cancellationToken)
        {
            var encoded = Encoding.UTF8.GetBytes(data);
            var buffer = new ArraySegment<byte>(encoded, 0, encoded.Length);
            await clientWebSocket_.SendAsync(buffer, WebSocketMessageType.Text, true, cancellationToken);
        }

        public async Task<string> ReceiveMessage()
        {
            var buffer = new ArraySegment<byte>(new byte[1024]);
            WebSocketReceiveResult result = null;
            var allBytes = new List<byte>();

            do
            {
                result = await clientWebSocket_.ReceiveAsync(buffer, CancellationToken.None);
                for (int i = 0; i < result.Count; i++)
                {
                    allBytes.Add(buffer.Array[i]);
                }
            }
            while (!result.EndOfMessage);

            var text = Encoding.UTF8.GetString(allBytes.ToArray(), 0, allBytes.Count);

            return text;
        }

        public void Close()
        {
            Dispose();
        }

        public void Dispose()
        {
            cancellationTokenSource.Dispose();
            clientWebSocket_.Dispose();
        }
        
        protected virtual void OnMessageReceived(string message)
        {
            MessageReceived?.Invoke(message);
        }
    }
}