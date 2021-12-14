using System.Collections.Generic;
using Newtonsoft.Json;

namespace Eyes.Universal.Sdk.DotNet.Commands.Client
{
    public abstract class ClientCommandBase
    {
        [JsonProperty("payload")]
        public IDictionary<string, object> Payload { get; }

        [JsonProperty("name")]
        public abstract string Name { get; }

        protected ClientCommandBase()
        {
            Payload = new Dictionary<string, object>();
        }

        public void AddOrUpdatePayload<T>(string key, T value)
        {
            Payload[key] = value;
        }
    }
}