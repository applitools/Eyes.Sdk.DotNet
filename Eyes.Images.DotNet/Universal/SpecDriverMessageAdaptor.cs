using Applitools.Commands;
using Newtonsoft.Json;

namespace Applitools
{
    public class SpecDriverMessageAdaptor : ISpecDriverMessageAdaptor
    {
        public TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase
        {
            return JsonConvert.DeserializeObject<TResponse>(message);
        }
    }
}