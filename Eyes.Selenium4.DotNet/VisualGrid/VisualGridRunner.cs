using Applitools.Commands.Requests;

namespace Applitools.VisualGrid
{
    public class VisualGridRunner : Selenium.SeleniumEyesRunner//, IVisualGridRunner
    {
        internal const int CONCURRENCY_FACTOR = 5;
        internal const int DEFAULT_CONCURRENCY = 5;
        internal readonly RunnerOptions runnerOptions_;
        
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
        
        public VisualGridRunner(string suiteName = null, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(DEFAULT_CONCURRENCY), suiteName, logHandler)
        {
            testConcurrency_ = new TestConcurrency();
        }

        public VisualGridRunner(int concurrentOpenSessions, ILogHandler logHandler = null)
            : this(concurrentOpenSessions, null, logHandler)
        { }

        public VisualGridRunner(int concurrentOpenSessions, string suiteName, ILogHandler logHandler = null)
            : this(new RunnerOptions().TestConcurrency(concurrentOpenSessions * CONCURRENCY_FACTOR), suiteName, logHandler)
        {
            testConcurrency_ = new TestConcurrency(concurrentOpenSessions, true);
        }

        public VisualGridRunner(RunnerOptions runnerOptions, ILogHandler logHandler = null)
            : this(runnerOptions, null, logHandler)
        { }

        public VisualGridRunner(RunnerOptions runnerOptions, string suiteName, ILogHandler logHandler = null) 
            : base(logHandler, "netSelenium4")
        {
            runnerOptions_ = runnerOptions;
            testConcurrency_ = new TestConcurrency(((IRunnerOptionsInternal)runnerOptions).GetConcurrency(), false);
            if (logHandler != null) Logger.SetLogHandler(logHandler);

            ManagerApplitoolsRefId = GetCoreMakeManager();
        }

        public VisualGridRunner(RunnerOptions runnerOptions, string suiteName,
            IServerConnectorFactory serverConnectorFactory, ILogHandler logHandler = null) 
            : base(logHandler, "netSelenium4")
        {
            runnerOptions_ = runnerOptions;
            testConcurrency_ = new TestConcurrency(((IRunnerOptionsInternal)runnerOptions).GetConcurrency(), false);
            if (logHandler != null) Logger.SetLogHandler(logHandler);

            ManagerApplitoolsRefId = GetCoreMakeManager();
        }

        internal VisualGridRunner(int concurrentOpenSessions, string suiteName, 
            IServerConnectorFactory serverConnectorFactory, ILogHandler logHandler = null)
            : this(new RunnerOptions(concurrentOpenSessions), suiteName, serverConnectorFactory, logHandler)
        {
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
    }
}
