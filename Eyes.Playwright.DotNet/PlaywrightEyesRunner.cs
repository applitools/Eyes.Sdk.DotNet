using System;
using Applitools.Commands.Requests;
using Applitools.Playwright.Universal.Driver;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.Playwright
{
    public abstract class PlaywrightEyesRunner : EyesRunner
    {
        /// <summary>
        /// version of the client sdk
        /// </summary>
        protected static string VERSION = typeof(ClassicRunner).Assembly.GetName().Version.ToString();

        /// <summary>
        /// spec-driver
        /// </summary>
        private static string[] COMMANDS = new[]
        {
            "getUrl",
            "getTitle",
            "getCookies",
            "isDriver",
            "isContext",
            "isElement",
            "isSelector",
            "getViewportSize",
            "findElement",
            "findElements",
            "takeScreenshot",
            "childContext",
            "mainContext",
            "parentContext",
            "setViewportSize",
            "getDriverInfo",
            "executeScript",
            "visit"
        };
        
        public Refer Refer { get; protected set; }

        protected PlaywrightEyesRunner(ILogHandler logHandler, string agentId) 
            : base(logHandler, agentId, PlaywrightSpecDriverMessageListener.Instance.Value)
        {
        }
        
        protected override CoreMakeSdkRequest CreateCoreMakeSdk(string name)
        {
            var result = new CoreMakeSdkRequest{
                Payload = new MakeCorePayload
                {
                    AgentId = $"{name}:{ActualAssembly.GetName().Version}",
                    Cwd = Environment.CurrentDirectory,
                    Commands = COMMANDS
                }
            };

            return result;
        }
    }
}