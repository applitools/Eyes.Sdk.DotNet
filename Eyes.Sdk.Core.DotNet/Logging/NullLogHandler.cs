namespace Applitools
{
    public class NullLogHandler : LogHandlerBase
    {
        public readonly static NullLogHandler Instance = new NullLogHandler();

        public NullLogHandler() : base(TraceLevel.None) { }

        public override void OnMessage(string message, TraceLevel level) { }
    }
}
