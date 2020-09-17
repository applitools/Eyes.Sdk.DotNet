using NUnit.Framework;
using System;

namespace Applitools.Tests.Utils
{
    public class NunitLogHandler : ILogHandler
    {
        private readonly bool isVerbose_;

        /// <summary>
        /// Creates a new <see cref="NunitLogHandler"/> instance.
        /// </summary>
        /// <param name="isVerbose">Whether to handle or ignore verbose log messages.</param>
        public NunitLogHandler(bool isVerbose)
        {
            isVerbose_ = isVerbose;
        }

        /// <summary>
        /// Creates a new <see cref="NunitLogHandler"/> that ignores verbose log messages.
        /// </summary>
        public NunitLogHandler()
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
                TestContext.Progress.WriteLine(DateTimeOffset.Now + " - Eyes: " + message);
            }
        }

        public void OnMessage(bool verbose, Func<string> messageProvider)
        {
            if (!verbose || isVerbose_)
            {
                TestContext.Progress.WriteLine(messageProvider());
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