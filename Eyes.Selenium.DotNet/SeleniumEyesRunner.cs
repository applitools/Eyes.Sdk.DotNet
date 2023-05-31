using Applitools.Universal;

namespace Applitools.Selenium
{
    public abstract class SeleniumEyesRunner : EyesRunner
    {
        protected SeleniumEyesRunner(ILogHandler logHandler, string agentId)
            : base(logHandler, agentId, SpecDriverMessageListener.Instance.Value)
        {
        }
    }
}