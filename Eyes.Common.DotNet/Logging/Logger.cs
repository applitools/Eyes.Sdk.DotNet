﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Threading;
using Applitools.Utils;
using Applitools.VisualGrid;

namespace Applitools
{
    /// <summary>
    /// Logs trace messages.
    /// </summary>
    public class Logger
    {
        private ILogHandler logHandler_ = new NullLogHandler();
        private const string lineFormat = "{0:yyyy'-'MM'-'dd HH':'mm':'ss.fff} [{4,-7}] [{3,4}] Eyes: {1} {2}";

        public string AgentId { get; set; }

        /// <summary>
        /// Gets the log handler.
        /// </summary>
        public ILogHandler GetILogHandler()
        {
            return logHandler_;
        }

        /// <summary>
        /// Sets the log handler.
        /// </summary>
        /// <param name_="handler"></param>
        public void SetLogHandler(ILogHandler handler)
        {
            ArgumentGuard.NotNull(handler, nameof(handler));

            logHandler_ = handler;
        }

        /// <summary>
        /// Returns the name of the method which called the logger, if possible, or an empty string.
        /// </summary>
        /// <returns>The name of the method which called the logger, if possible, or an empty string.</returns>
        private string GetPrefix_()
        {
            // getStackTrace()<-getPrefix()<-log()/verbose()<-"actual caller"
            StackFrame stackFrame = new StackFrame(4);
            MethodBase method = stackFrame.GetMethod();
            string prefix = method.DeclaringType.Name + "." + method.Name + ": ";
            return prefix;
        }

        ///// <summary>
        ///// Writes a verbose log message.
        ///// </summary>
        ///// <param name="message">The message to write to the log.</param>
        ///// <param name="args">Optional arguments to place inside the message.</param>
        //[Conditional("DEBUG")]
        //public void Debug([Localizable(false)] string message, params object[] args)
        //{
        //    logHandler_.OnMessage(TraceLevel.Debug, () =>
        //    {
        //        if (args != null && args.Length > 0)
        //        {
        //            message = string.Format(message, args);
        //        }
        //        return string.Format(lineFormat, DateTimeOffset.Now, GetPrefix_(), message, Thread.CurrentThread.ManagedThreadId, "DEBUG");
        //    });
        //}

        /// <summary>
        /// Writes a verbose log message.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args">Optional arguments to place inside the message.</param>
        public void Verbose([Localizable(false)] string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            LogInner_(TraceLevel.Info, null, Stage.General, null, new { message });
        }

        /// <summary>
        /// Writes a (non-verbose) log message.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args">Optional arguments to place inside the message.</param>
        public void Log(string message, params object[] args)
        {
            if (args != null && args.Length > 0)
            {
                message = string.Format(message, args);
            }
            LogInner_(TraceLevel.Notice, null, Stage.General, null, new { message });
        }

        public void Log(Stage stage, StageType type)
        {
            LogInner_(TraceLevel.Notice, null, stage, type, null);
        }

        public void Log(TraceLevel level, IEnumerable<string> testIds, Stage stage, object data = null)
        {
            LogInner_(level, testIds, stage, null, data);
        }

        public void Log(TraceLevel level, IEnumerable<string> testIds, Stage stage, StageType type, object data = null)
        {
            LogInner_(level, testIds, stage, type, data);
        }

        public void Log(TraceLevel level, string testId, Stage stage, object data = null)
        {
            LogInner_(level, testId == null ? null : new string[] { testId }, stage, null, data);
        }

        public void Log(TraceLevel level, string testId, Stage stage, StageType type, object data = null)
        {
            LogInner_(level, testId == null ? null : new string[] { testId }, stage, type, data);
        }

        public void Log(TraceLevel level, Stage stage, object data = null)
        {
            LogInner_(level, null, stage, null, data);
        }

        public void Log(TraceLevel level, Stage stage, StageType? type = null, object data = null)
        {
            LogInner_(level, null, stage, type, data);
        }

        private void LogInner_(TraceLevel level, IEnumerable<string> testIds, Stage stage, StageType? type, object data)
        {
            string currentTime = DateTimeOffset.UtcNow.ToString(StandardDateTimeFormat.ISO8601);
            ClientEvent @event = new ClientEvent(currentTime, CreateMessageFromLog(testIds, stage, type, 3, data), level);
            logHandler_.OnMessage(@event);
        }

        private Message CreateMessageFromLog(IEnumerable<string> testIds, Stage stage, StageType? type, int methodsBack,
            object data)
        {
            StackFrame[] stackFrames = new StackTrace().GetFrames();
            string stackTrace = "";
            if (stackFrames.Length > methodsBack)
            {
                MethodBase method = stackFrames[methodsBack].GetMethod();
                stackTrace += $"{method.DeclaringType.Name}.{method.Name}()";
            }

            return new Message(AgentId, stage, type, testIds, Thread.CurrentThread.ManagedThreadId, stackTrace, data);
        }

    }

}
