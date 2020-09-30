using System;
using System.Runtime.InteropServices;

namespace Applitools
{
    /// <summary>
    /// Writes log messages to the standard output stream.
    /// </summary>
    [ComVisible(true)]
    public class StdoutLogHandler : ILogHandler
    {
        private readonly bool isVerbose_;

        /// <summary>
        /// Creates a new <see cref="StdoutLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public StdoutLogHandler(bool isVerbose)
        {
            isVerbose_ = isVerbose;
        }

        /// <summary>
        /// Creates a new <see cref="StdoutLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public StdoutLogHandler()
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
                Console.WriteLine(DateTimeOffset.Now + " - Eyes: " + message);
            }
        }

        public void OnMessage(bool verbose, Func<string> messageProvider)
        {
            if (!verbose || isVerbose_)
            {
                Console.WriteLine(messageProvider());
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
