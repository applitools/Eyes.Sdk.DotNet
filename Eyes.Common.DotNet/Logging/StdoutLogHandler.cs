using System;
using System.Runtime.InteropServices;

namespace Applitools
{
    /// <summary>
    /// Writes log messages to the standard output stream.
    /// </summary>
    [ComVisible(true)]
    public class StdoutLogHandler : LogHandlerBase
    {
        /// <summary>
        /// Creates a new <see cref="StdoutLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public StdoutLogHandler(bool isVerbose) : base(isVerbose) { isOpen_ = true; }

        /// <summary>
        /// Creates a new <see cref="StdoutLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public StdoutLogHandler() : this(true) { }

        public override void OnMessage(string message, TraceLevel level)
        {
            Console.WriteLine(message);
        }
    }
}
