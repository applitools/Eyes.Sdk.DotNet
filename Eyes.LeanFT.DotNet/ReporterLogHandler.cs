namespace Applitools
{
    using HP.LFT.Report;
    using System;

    public class ReporterLogHandler : ILogHandler
    {
        public static readonly string FAILED_STR = "Failed test ended";
        public static readonly string EXCEPTION_STR = "Exception";
        public static readonly string NEW_STR = "New test ended";

        #region Constructors

        /// <summary>
        /// Creates a new <see cref="ReporterLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public ReporterLogHandler(IReporter reporter, bool isVerbose)
        {
            IsVerbose = isVerbose;
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
        /// Whether to handle or ignore verbose log messages.
        /// </summary>
        public bool IsVerbose { get; set; }

        /// <summary>
        /// The path to the log file.
        /// </summary>
        public IReporter Reporter { get; private set; }

        public bool IsOpen => true;

        #endregion

        #region Methods

        public void OnMessage(bool verbose, string message, params object[] args)
        {
            try
            {
                if (!verbose || IsVerbose)
                {
                    if (args != null && args.Length > 0)
                    {
                        message = string.Format(message, args);
                    }
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
            }
            catch
            {
                // We don't want a trace failure the fail the test
            }
        }

        public void OnMessage(bool verbose, Func<string> messageProvider)
        {
            if (!verbose || IsVerbose)
            {
                string message = messageProvider();
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
        }

        public void Open()
        {
        }

        public void Close()
        {
        }

        #endregion
    }
}
