using Applitools.Commands.Requests;

namespace Applitools.Appium
{
    public class ClassicRunner : AppiumEyesRunner
    {
        public EyesException Exception { get; set; }
        
        public ClassicRunner() : this(NullLogHandler.Instance) { }

        public ClassicRunner(ILogHandler logHandler) : this(logHandler, null)
        {
        }

        internal ClassicRunner(ILogHandler logHandler, IServerConnectorFactory serverConnectorFactory)
            : base(logHandler, "Eyes.Appium.DotNet")
        {
            ManagerApplitoolsRefId = GetCoreMakeManager();
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
