using System.Collections.Generic;
using Applitools.Selenium.Commands.Requests;

namespace Applitools
{
    public class ClassicRunner : EyesRunner
    {
        private EyesBase eyes_;

        public EyesException Exception { get; set; }

        internal void SetEyes(EyesBase eyes)
        {
            eyes_ = eyes;
        }

        public ClassicRunner(ILogHandler logHandler) : base(logHandler)
        {
        }

        internal ClassicRunner(ILogHandler logHandler, IServerConnectorFactory serverConnectorFactory) : base(logHandler)
        {
        }

        public ClassicRunner() : this(NullLogHandler.Instance) { }

        protected override ManagerConfig InitConfig()
        {
            return new ManagerConfig
            {
                Type = "classic"
            };
        }

        protected override IEnumerable<IEyesBase> GetAllEyes()
        {
            throw new System.NotImplementedException();
        }
    }
}
