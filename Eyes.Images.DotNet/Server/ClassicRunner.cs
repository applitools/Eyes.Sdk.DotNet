using Applitools.Commands.Requests;

namespace Applitools
{
    public class ClassicRunner : EyesRunner
    {
        public EyesException Exception { get; set; }

        public ClassicRunner() : this(true) { }

        public ClassicRunner(bool startServer = true) : this(NullLogHandler.Instance) { }

        public ClassicRunner(ILogHandler logHandler) : this(logHandler, null)
        {
        }

        internal ClassicRunner(ILogHandler logHandler, IServerConnectorFactory serverConnectorFactory, bool startServer = true)
            : base(logHandler, "netImages")
        {
        }

        protected override MakeManagerRequestPayload InitConfig()
        {
            return new MakeManagerRequestPayload
            {
                Type = "classic"
            };
        }
    }
}
