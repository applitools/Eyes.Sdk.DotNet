using Applitools.Commands.Requests;

namespace Applitools
{
    public class ClassicRunner : EyesRunner
    {
        public EyesException Exception { get; set; }

        public ClassicRunner() : this(true) { }

        public ClassicRunner(bool startServer = true) : this(NullLogHandler.Instance, startServer) { }

        public ClassicRunner(ILogHandler logHandler, bool startServer = true) : this(logHandler, null, startServer)
        {
        }

        internal ClassicRunner(ILogHandler logHandler, IServerConnectorFactory serverConnectorFactory, bool startServer = true)
            : base(logHandler, startServer)
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
