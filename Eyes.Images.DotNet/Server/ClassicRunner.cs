using Applitools.Commands.Requests;
using Applitools.Universal;

namespace Applitools.Images
{
    public class ClassicRunner : EyesRunner
    {
        public EyesException Exception { get; set; }
        
        public ClassicRunner() : this(NullLogHandler.Instance) { }

        public ClassicRunner(ILogHandler logHandler) : this(logHandler, null)
        {
        }

        internal ClassicRunner(ILogHandler logHandler, IServerConnectorFactory serverConnectorFactory)
            : base(logHandler, "Eyes.Images.DotNet", SpecDriverMessageListener.Instance.Value)
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
