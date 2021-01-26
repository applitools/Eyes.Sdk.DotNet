using Applitools.Utils;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace Applitools
{
    public class LogSessionsClientEvents
    {
        [JsonIgnore]
        public int Count => Events.Count;

        public List<ClientEvent> Events { get; } = new List<ClientEvent>();

        public void Add(ClientEvent @event)
        {
            ArgumentGuard.NotNull(@event, nameof(@event));
            Events.Add(@event);
        }

        public void Clear()
        {
            Events.Clear();
        }
    }
}
