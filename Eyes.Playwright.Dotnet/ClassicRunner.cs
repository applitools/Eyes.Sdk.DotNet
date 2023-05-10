using System;
using Applitools.Playwright.Universal;
using System.Diagnostics;
using Applitools.Commands.Requests;
using Applitools.Playwright.Universal.Driver;

namespace Applitools.Playwright
{
    public class ClassicRunner : EyesRunner
    {
        /// <summary>
        /// name of the client sdk
        /// </summary>
        protected static string BASE_AGENT_ID = "Eyes.Playwright.DotNet";

        /// <summary>
        /// version of the client sdk
        /// </summary>
        protected static string VERSION = FileVersionInfo.GetVersionInfo(typeof(ClassicRunner).Assembly.Location).FileVersion;

        /// <summary>
        /// spec-driver
        /// </summary>
        protected static string[] COMMANDS = SpecDriverPlaywright.GetMethodNames();

        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        public ClassicRunner() : this(BASE_AGENT_ID, VERSION)
        {
        }

        /// <summary>
        /// used for instantiating Playwright Runner
        /// </summary>
        public ClassicRunner(string baseAgentId, string version)
            : base(baseAgentId, version, null, COMMANDS, listener_)
        {
            managerRef = commandExecutor.coreMakeManager("classic", null);
        }

        public PlaywrightStaleElementReferenceException GetStaleElementException()
        {
            return new PlaywrightStaleElementReferenceException();
        }
        
        protected override CoreMakeSdkRequest CreateCoreMakeSdk(string name)
        {
            var result = new CoreMakeSdkRequest();
            result.AddOrUpdatePayload("agentId", $"{name}:{ActualAssembly.GetName().Version}");
            result.AddOrUpdatePayload("spec", "playwright");
            result.AddOrUpdatePayload("cwd", Environment.CurrentDirectory);

            return result;
        }

        protected Refer GtRefer()
        {
            return listener_.getRefer();
        }
    }
}