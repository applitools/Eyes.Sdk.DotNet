namespace Applitools
{
    using System;
    using System.Runtime.InteropServices;
    using Applitools.Utils;

    /// <summary>
    /// Writes log messages to <see cref="System.Diagnostics.Trace"/>.
    /// </summary>
    [ComVisible(true)]
    public class TraceLogHandler : ILogHandler
    {
        private readonly bool isVerbose_;

        /// <summary>
        /// Creates a new <see cref="TraceLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public TraceLogHandler(bool isVerbose)
        {
            isVerbose_ = isVerbose;
        }

        /// <summary>
        /// Creates a new <see cref="TraceLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public TraceLogHandler()
            : this(true)
        {
        }

        public void OnMessage(bool verbose, string message, params object[] args)
        {
            if (!verbose || isVerbose_)
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                System.Diagnostics.Trace.WriteLine("{0} - Eyes: {1}".Fmt(DateTimeOffset.Now, message));
            }
        }

        public void OnMessage(bool verbose, Func<string> messageProvider)
        {
            if (!verbose || isVerbose_)
            {
                System.Diagnostics.Trace.WriteLine(messageProvider());
            }
        }
        public bool IsOpen => true;

        public void Open()
        {
        }

        public void Close()
        {
        }

        public void Dispose()
        {
        }
    }
}
