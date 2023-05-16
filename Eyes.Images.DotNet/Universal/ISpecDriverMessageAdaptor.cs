using Applitools.Commands;

namespace Applitools.Universal
{
    public interface ISpecDriverMessageAdaptor
    {
        TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase;
        WebSocketClient WebSocket { get; set; }
    }
}