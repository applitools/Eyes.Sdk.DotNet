using System;

namespace Applitools.Selenium
{
    public interface IUniversalSocketClient : IDisposable
    {
        event DataReceived OnDataReceived;
        bool Connect();
        bool SendData(SocketMessage message);
        void Close();
    }
}