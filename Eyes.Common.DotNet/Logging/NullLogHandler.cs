namespace Applitools
{
    public class NullLogHandler : LogHandlerBase
    {
        public static NullLogHandler Instance = new NullLogHandler();

        public NullLogHandler() : base(false) { }

        public override void OnMessage(string message, TraceLevel level) { }
    }
}
