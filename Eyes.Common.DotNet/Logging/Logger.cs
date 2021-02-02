using System;
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

        /// <summary>
        /// Writes a verbose log message.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args">Optional arguments to place inside the message.</param>
        [Conditional("DEBUG")]
        public void Debug([Localizable(false)] string message, params object[] args)
        {
            logHandler_.OnMessage(TraceLevel.Debug, () =>
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                return string.Format(lineFormat, DateTimeOffset.Now, GetPrefix_(), message, Thread.CurrentThread.ManagedThreadId, "DEBUG");
            });
        }

        /// <summary>
        /// Writes a verbose log message.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args">Optional arguments to place inside the message.</param>
        public void Verbose([Localizable(false)] string message, params object[] args)
        {
            logHandler_.OnMessage(TraceLevel.Info, () =>
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                return string.Format(lineFormat, DateTimeOffset.Now, GetPrefix_(), message, Thread.CurrentThread.ManagedThreadId, "VERBOSE");
            });
        }

        /// <summary>
        /// Writes a (non-verbose) log message.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args">Optional arguments to place inside the message.</param>
        public void Log(string message, params object[] args)
        {
            logHandler_.OnMessage(TraceLevel.Notice, () =>
            {
                if (args != null && args.Length > 0)
                {
                    message = string.Format(message, args);
                }
                return string.Format(lineFormat, DateTimeOffset.Now, GetPrefix_(), message, Thread.CurrentThread.ManagedThreadId, "LOG");
            });
        }

        public void Log(Stage stage, StageType type, params Tuple<string, object>[] data)
        {
            LogInner_(TraceLevel.Notice, null, stage, type, data);
        }

        public void Log(TraceLevel level, HashSet<string> testIds, Stage stage, StageType? type,
            params Tuple<string, object>[] data)
        {
            LogInner_(level, testIds, stage, type, data);
        }

        public void Log(TraceLevel level, string testId, Stage stage, params Tuple<string, object>[] data)
        {
            LogInner_(level, testId == null ? null : new HashSet<string>(new string[] { testId }), stage, type: null, data);
        }

        public void Log(TraceLevel level, string testId, Stage stage, StageType type, params Tuple<string, object>[] data)
        {
            LogInner_(level, testId == null ? null : new HashSet<string>(new string[] { testId }), stage, type, data);
        }

        private void LogInner_(TraceLevel level, HashSet<string> testIds, Stage stage, StageType? type,
            params Tuple<string, object>[] data)
        {
            string currentTime = DateTimeOffset.UtcNow.ToString(StandardDateTimeFormat.ISO8601);
            ClientEvent @event = new ClientEvent(currentTime, CreateMessageFromLog(testIds, stage, type, 4, data), level);
            logHandler_.OnMessage(@event);
        }

        private Message CreateMessageFromLog(HashSet<string> testIds, Stage stage, StageType? type, int methodsBack,
            Tuple<string, object>[] data)
        {
            Dictionary<string, object> map = new Dictionary<string, object>();
            if (data != null && data.Length > 0)
            {
                foreach (Tuple<string, object> pair in data)
                {
                    map[pair.Item1] = pair.Item2;
                }
            }

            StackFrame[] stackFrames = new StackTrace().GetFrames();
            string stackTrace = "";
            if (stackFrames.Length > methodsBack)
            {
                MethodBase method = stackFrames[methodsBack].GetMethod();
                stackTrace += $"{method.DeclaringType.Name}.{method.Name}()";
            }

            return new Message(AgentId, stage, type, testIds, Thread.CurrentThread.ManagedThreadId, stackTrace, map);
        }

    }

}
