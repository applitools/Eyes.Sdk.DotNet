using System;

namespace Eyes.Universal.Sdk.DotNet
{
    public interface IUniversalSocketClient : IDisposable
    {
        event DataReceived OnDataReceived;
        bool Connect();
        bool SendData(SocketMessage message);
        void Close();
    }
}