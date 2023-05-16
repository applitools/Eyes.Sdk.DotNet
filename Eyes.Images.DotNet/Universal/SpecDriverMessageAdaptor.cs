using Applitools.Commands;
using Newtonsoft.Json;

namespace Applitools.Universal
{
    public class SpecDriverMessageAdaptor : ISpecDriverMessageAdaptor
    {
        public TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase
        {
            return JsonConvert.DeserializeObject<TResponse>(message);
        }

        public WebSocketClient WebSocket { get; set; }
    }
}