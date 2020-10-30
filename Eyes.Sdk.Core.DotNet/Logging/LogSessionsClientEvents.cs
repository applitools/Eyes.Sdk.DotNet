using Applitools.Utils;
using System.Collections;
using System.Collections.Generic;

namespace Applitools
{
    public class LogSessionsClientEvents : ICollection<ClientEvent>
    {
        private readonly List<ClientEvent> events_ = new List<ClientEvent>();

        public int Count => events_.Count;

        public bool IsReadOnly => false;

        public List<ClientEvent> GetEvents()
        {
            return events_;
        }

        public void Add(ClientEvent @event)
        {
            ArgumentGuard.NotNull(@event, nameof(@event));
            events_.Add(@event);
        }

        public void Clear()
        {
            events_.Clear();
        }

        public bool Contains(ClientEvent @event)
        {
            return events_.Contains(@event);
        }

        public void CopyTo(ClientEvent[] array, int arrayIndex)
        {
            events_.CopyTo(array, arrayIndex);
        }

        public bool Remove(ClientEvent item)
        {
            return events_.Remove(item);
        }

        public IEnumerator<ClientEvent> GetEnumerator()
        {
            return events_.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return events_.GetEnumerator();
        }
    }
}
