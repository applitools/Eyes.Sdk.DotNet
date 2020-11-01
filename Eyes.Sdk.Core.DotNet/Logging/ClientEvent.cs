namespace Applitools
{
    public class ClientEvent
    {
        public ClientEvent(string timestamp, string @event) : this(timestamp, @event, default) { }

        public ClientEvent(string timestamp, string @event, TraceLevel level)
        {
            Timestamp = timestamp;
            Event = @event;
            Level = level;
        }

        public string Timestamp { get; private set; }
        public string Event { get; }
        public TraceLevel Level { get; }
    }
}