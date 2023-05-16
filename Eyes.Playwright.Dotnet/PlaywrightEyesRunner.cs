namespace Applitools.Playwright
{
    public abstract class PlaywrightEyesRunner : EyesRunner
    {
        protected PlaywrightEyesRunner(ILogHandler logHandler, string agentId) : base(logHandler, agentId)
        {
        }
    }
}