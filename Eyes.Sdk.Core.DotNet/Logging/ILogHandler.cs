namespace Applitools
{
    using System;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Handles log messages produces by the Eyes API.
    /// </summary>
    [ComVisible(true)]
    public interface ILogHandler
    {
        bool IsOpen { get; }

        /// <summary>
        /// Invoked when a new test starts (after Eyes.Open is called)
        /// </summary>
        void Open();

        /// <summary>
        /// Invoked when a test ends.
        /// </summary>
        void Close();

        /// <summary>
        /// Invoked when a log message is emitted.
        /// </summary>
        void OnMessage(bool verbose, string message, params object[] args);

        void OnMessage(bool verbose, Func<string> messageProvider);
    }
}
