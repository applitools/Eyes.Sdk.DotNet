using System.Runtime.InteropServices;

namespace Applitools
{
    /// <summary>
    /// Writes log messages to <see cref="System.Diagnostics.Trace"/>.
    /// </summary>
    [ComVisible(true)]
    public class TraceLogHandler : LogHandlerBase
    {
        /// <summary>
        /// Creates a new <see cref="TraceLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public TraceLogHandler(bool isVerbose) : base(isVerbose) { isOpen_ = true; }

        /// <summary>
        /// Creates a new <see cref="TraceLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public TraceLogHandler() : this(true) { }

        public override void OnMessage(string message, TraceLevel level)
        {
            System.Diagnostics.Trace.WriteLine(message);
        }
    }
}
