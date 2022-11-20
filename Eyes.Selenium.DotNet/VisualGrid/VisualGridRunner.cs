using Applitools.Selenium.Commands.Requests;

namespace Applitools.VisualGrid
{
    public class VisualGridRunner : EyesRunner//, IVisualGridRunner
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

        public VisualGridRunner(string suiteName = null, ILogHandler logHandler = null, bool startServer = true)
            : this(new RunnerOptions().TestConcurrency(DEFAULT_CONCURRENCY), suiteName, logHandler, startServer)
        {
            testConcurrency_ = new TestConcurrency();
        }

        public VisualGridRunner(int concurrentOpenSessions, ILogHandler logHandler = null, bool startServer = true)
            : this(concurrentOpenSessions, null, logHandler, startServer)
        { }

        public VisualGridRunner(int concurrentOpenSessions, string suiteName, ILogHandler logHandler = null, bool startServer = true)
            : this(new RunnerOptions().TestConcurrency(concurrentOpenSessions * CONCURRENCY_FACTOR), suiteName, logHandler, startServer)
        {
            testConcurrency_ = new TestConcurrency(concurrentOpenSessions, true);
        }

        public VisualGridRunner(RunnerOptions runnerOptions, ILogHandler logHandler = null, bool startServer = true)
            : this(runnerOptions, null, logHandler, startServer)
        { }

        public VisualGridRunner(RunnerOptions runnerOptions, string suiteName, ILogHandler logHandler = null, bool startServer = true)
            : base(logHandler, startServer)
        {
            runnerOptions_ = runnerOptions;
            testConcurrency_ = new TestConcurrency(((IRunnerOptionsInternal)runnerOptions).GetConcurrency(), false);
            if (logHandler != null) Logger.SetLogHandler(logHandler);
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
