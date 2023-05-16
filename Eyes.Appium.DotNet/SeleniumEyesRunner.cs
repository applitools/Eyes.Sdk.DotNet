namespace Applitools.Appium
{
    public abstract class AppiumEyesRunner : Selenium.SeleniumEyesRunner
    {
        protected AppiumEyesRunner(ILogHandler logHandler, string agentId) : base(logHandler, agentId)
        {
        }
    }
}