using System;
using Applitools.Commands.Requests;
using Applitools.Playwright;
using Applitools.Playwright.Universal;
using Applitools.Playwright.Universal.Driver;
using Applitools.Universal;
using Refer = Applitools.Playwright.Universal.Refer;

namespace Applitools.VisualGrid
{
    /// <summary>
    /// Used to manage multiple Eyes sessions when working with the Ultrafast Grid
    /// </summary>
    public class VisualGridRunner : PlaywrightEyesRunner
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

        private readonly RunnerOptions runnerOptions_;
        private readonly SpecDriverPlaywright driver_;

        public VisualGridRunner(string suiteName = null, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(DEFAULT_CONCURRENCY), suiteName, logHandler)
        {
        }

        public VisualGridRunner(int concurrentOpenSessions, ILogHandler logHandler = null)
            : this(concurrentOpenSessions, null, logHandler)
        { }

        public VisualGridRunner(int concurrentOpenSessions, string suiteName, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(concurrentOpenSessions * CONCURRENCY_FACTOR), suiteName, logHandler)
        {
        }

        public VisualGridRunner(RunnerOptions runnerOptions, ILogHandler logHandler = null)
            : this(runnerOptions, null, logHandler)
        { }

        public VisualGridRunner(RunnerOptions runnerOptions, string suiteName, ILogHandler logHandler = null) 
            : base(logHandler, BASE_AGENT_ID)
        {
            runnerOptions_ = runnerOptions;
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }

            ManagerApplitoolsRefId = GetCoreMakeManager();
            Refer = new Refer();
            driver_ = new SpecDriverPlaywright(Refer);
        }

        public VisualGridRunner(RunnerOptions runnerOptions, string suiteName,
            IServerConnectorFactory serverConnectorFactory, ILogHandler logHandler = null) 
            : base(logHandler, BASE_AGENT_ID)
        {
            runnerOptions_ = runnerOptions;
            if (logHandler != null)
            {
                Logger.SetLogHandler(logHandler);
            }
            Refer = new Refer();
            driver_ = new SpecDriverPlaywright(Refer);
            ManagerApplitoolsRefId = GetCoreMakeManager();
        }

        internal VisualGridRunner(int concurrentOpenSessions, string suiteName, 
            IServerConnectorFactory serverConnectorFactory, ILogHandler logHandler = null)
            : this(new RunnerOptions(concurrentOpenSessions), suiteName, serverConnectorFactory, logHandler)
        {
        }

        public PlaywrightStaleElementReferenceException GetStaleElementException()
        {
            return new PlaywrightStaleElementReferenceException();
        }

        protected override MakeManagerRequestPayload InitConfig()
        {
            return new MakeManagerRequestPayload
            {
                Type = "ufg",
                Concurrency = ((IRunnerOptionsInternal)runnerOptions_).GetConcurrency(),
                Legacy = false
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

        private static PlaywrightSpecDriverMessageAdaptor messageAdaptor_ = null;

        protected override ISpecDriverMessageAdaptor SpecDriverMessageAdaptor
        {
            get
            {
                if (messageAdaptor_ == null)
                {
                    messageAdaptor_ = new PlaywrightSpecDriverMessageAdaptor(Refer, Logger);
                }

                return messageAdaptor_;
            }
        }
    }
}