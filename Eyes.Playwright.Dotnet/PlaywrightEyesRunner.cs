using Applitools.Playwright.Universal;

namespace Applitools.Playwright
{
    public abstract class PlaywrightEyesRunner : EyesRunner
    {
        public Refer Refer { get; set; }

        protected PlaywrightEyesRunner(ILogHandler logHandler, string agentId) : base(logHandler, agentId)
        {
        }
    }
}