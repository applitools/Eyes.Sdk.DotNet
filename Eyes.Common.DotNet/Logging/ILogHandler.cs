﻿using System;
using System.Runtime.InteropServices;

namespace Applitools
{
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
        void OnMessage(TraceLevel level, string message, params object[] args);

        void OnMessage(TraceLevel level, Func<string> messageProvider);

        void OnMessage(ClientEvent @event);
    }
}
