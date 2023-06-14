using System.Collections.Concurrent;

namespace Applitools.Universal
{
    public interface ISpecDriverMessageListener
    {
        void HandleMessage(string message);
        WebSocketClient WebSocket { get; set; }
        void SetMessagesDictionary(ConcurrentDictionary<string,WsMessage> messages);
        string Protocol { get; }
        Logger Logger { get; }
    }
}