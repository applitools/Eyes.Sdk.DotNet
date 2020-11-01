using NUnit.Framework;

namespace Applitools.Tests.Utils
{
    public class NunitLogHandler : LogHandlerBase
    {
        /// <summary>
        /// Creates a new <see cref="NunitLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public NunitLogHandler(bool isVerbose) : base(isVerbose) { isOpen_ = true; }

        /// <summary>
        /// Creates a new <see cref="NunitLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public NunitLogHandler() : this(true) { }

        public override void OnMessage(string message, TraceLevel level)
        {
            TestContext.Progress.WriteLine(message);
        }
    }
}