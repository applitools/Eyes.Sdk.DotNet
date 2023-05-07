using System;
using System.Reflection;
using Applitools.Playwright.Universal;
using System.Diagnostics;

namespace Applitools.Playwright
{
    public class ClassicRunner : EyesRunner
    {
        /// <summary>
        /// name of the client sdk
        /// </summary>
        protected static string BASE_AGENT_ID = "eyes.playwright.java";

        /// <summary>
        /// version of the client sdk
        /// </summary>
        protected static string VERSION = FileVersionInfo.GetVersionInfo(typeof(ClassicRunner).Assembly.Location).FileVersion;

        /// <summary>
        /// spec-driver
        /// </summary>
        protected static string[] COMMANDS = SpecDriverPlaywright.getMethodNames();

        /// <summary>
        /// universal server listener
        /// </summary>
        private static readonly PSDKListener listener_ = PSDKListener.getInstance();

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

        public StaleElementReferenceException getStaleElementException()
        {
            return new PlaywrightStaleElementReferenceException();
        }

        protected Refer getRefer()
        {
            return listener_.getRefer();
        }
    }
}