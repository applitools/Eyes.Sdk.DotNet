using System;
using Applitools.Commands.Requests;
using Applitools.Playwright.Universal;
using Applitools.Playwright.Universal.Driver;

namespace Applitools.VisualGrid
{
    /// <summary>
    /// Used to manage multiple Eyes sessions when working with the Ultrafast Grid
    /// </summary>
    public class VisualGridRunner : EyesRunner
    {
        /// <summary>
        /// name of the client sdk
        /// </summary>
        protected static string BASE_AGENT_ID = "Eyes.Playwright.DotNet.VisualGrid";

        /// <summary>
        /// version of the client sdk
        /// </summary>
        protected static string VERSION = typeof(VisualGridRunner).Assembly.GetName().Version.ToString();

        /// <summary>
        /// spec-driver
        /// </summary>
        protected static string[] COMMANDS = SpecDriverPlaywright.GetMethodNames();
        
        //private static readonly PSdkListener listener = PSdkListener.Instance;
        
        internal const int CONCURRENCY_FACTOR = 5;
        static readonly int DEFAULT_CONCURRENCY = 5;

        private RunnerOptions runnerOptions;
      

        internal class TestConcurrency
        {
            public int UserConcurrency { get; }
            public int ActualConcurrency { get; }
            public bool IsLegacy { get; }
            public bool IsDefault { get; }

            public TestConcurrency()
            {
                IsDefault = true;
                IsLegacy = false;
                UserConcurrency = DEFAULT_CONCURRENCY;
                ActualConcurrency = DEFAULT_CONCURRENCY;
            }

            public TestConcurrency(int userConcurrency, bool isLegacy)
            {
                UserConcurrency = userConcurrency;
                ActualConcurrency = isLegacy ? userConcurrency * CONCURRENCY_FACTOR : userConcurrency;
                IsLegacy = isLegacy;
                IsDefault = false;
            }
        }

        internal readonly TestConcurrency testConcurrency_;

        public VisualGridRunner() :
            this(new RunnerOptions().TestConcurrency(DEFAULT_CONCURRENCY))
        {
        }

        public VisualGridRunner(int testConcurrency)
            : base(BASE_AGENT_ID, VERSION, null, COMMANDS, listener)
        {
            this.runnerOptions = new RunnerOptions().testConcurrency(testConcurrency);
            EyesManagerSettings managerSettings = new EyesManagerSettings(null, testConcurrency, null);
            managerRef = commandExecutor.coreMakeManager(ManagerType.VISUAL_GRID.value, managerSettings);
        }

        public VisualGridRunner(RunnerOptions runnerOptions)
            : base(BASE_AGENT_ID, VERSION, null, COMMANDS, listener, runnerOptions);
        {
            this.runnerOptions = runnerOptions;
            int testConcurrency = runnerOptions.getTestConcurrency() == null
                ? DEFAULT_CONCURRENCY
                : runnerOptions.getTestConcurrency();
            EyesManagerSettings managerSettings = new EyesManagerSettings(testConcurrency, null, null);
            managerRef = commandExecutor.coreMakeManager(ManagerType.VISUAL_GRID.value, managerSettings);
        }

        public StaleElementReferenceException getStaleElementException()
        {
            return new PlaywrightStaleElementReferenceException();
        }

        protected override MakeManagerRequestPayload InitConfig()
        {
            return new MakeManagerRequestPayload
            {
                Type = "ufg",
                Concurrency = testConcurrency_.UserConcurrency,
                Legacy = testConcurrency_.IsLegacy
            };
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