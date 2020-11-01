namespace Applitools
{
    public class ClientEvent
    {
        public ClientEvent(string timestamp, object @event) : this(timestamp, @event, default) { }

        public ClientEvent(string timestamp, object @event, TraceLevel level)
        {
            Timestamp = timestamp;
            Event = @event;
            Level = level;
        }

        public string Timestamp { get; private set; }
        public object Event { get; }
        public TraceLevel Level { get; }
    }
}