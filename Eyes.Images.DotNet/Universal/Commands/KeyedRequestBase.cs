using System.Collections.Generic;
using Newtonsoft.Json;

namespace Applitools.Commands
{
    public abstract class KeyedRequestBase : CommandBase
    {
        [JsonProperty("payload")]
        public IDictionary<string, object> Payload { get; }

        protected KeyedRequestBase()
        {
            Payload = new Dictionary<string, object>();
        }

        public void AddOrUpdatePayload<T>(string key, T value)
        {
            Payload[key] = value;
        }
    }
}