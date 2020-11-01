using HP.LFT.Report;

namespace Applitools
{
    public class ReporterLogHandler : LogHandlerBase
    {
        public static readonly string FAILED_STR = "Failed test ended";
        public static readonly string EXCEPTION_STR = "Exception";
        public static readonly string NEW_STR = "New test ended";

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ReporterLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public ReporterLogHandler(IReporter reporter, bool isVerbose) : base(isVerbose)
        {
            isOpen_ = true;
            Reporter = reporter;
        }

        /// <summary>
        /// Creates a new <see cref="ReporterLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public ReporterLogHandler(IReporter reporter)
            : this(reporter, true)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// The path to the log file.
        /// </summary>
        public IReporter Reporter { get; private set; }

        #endregion

        #region Methods

        public override void OnMessage(string message, TraceLevel level)
        {
            try
            {
                Status status = Status.Passed;
                if (message.Contains(FAILED_STR) || message.Contains(EXCEPTION_STR))
                {
                    status = Status.Failed;
                }
                else if (message.Contains(NEW_STR))
                {
                    status = Status.Warning;
                }

                Reporter.ReportEvent("Applitools Eyes", message, status);
            }
            catch
            {
                // We don't want a trace failure the fail the test
            }
        }

        #endregion
    }
}
