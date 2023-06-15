using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Reflection;
using System.Text;
using System.Threading;
using Applitools.Utils;

namespace Applitools
{
    /// <summary>
    /// Logs trace messages.
    /// </summary>
    public class Logger
    {
        private ILogHandler logHandler_ = new NullLogHandler();
        private bool skipLog_ = true;
        private ConcurrentDictionary<MethodBase, string> methodsCache_ = new ConcurrentDictionary<MethodBase, string>();
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
            skipLog_ = logHandler_ is NullLogHandler;
        }

        /// <summary>
        /// Writes a verbose log message.
        /// </summary>
        /// <param name="message">The message to write to the log.</param>
        /// <param name="args">Optional arguments to place inside the message.</param>
        public void Verbose([Localizable(false)] string message, params object[] args)
        {
            if (skipLog_) return;
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
            if (skipLog_) return;
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

        public void Log(TraceLevel level, IEnumerable<string> testIds, Stage stage, object data = null, int methodsBack = 3)
        {
            LogInner_(level, testIds, stage, null, data, methodsBack);
        }

        public void Log(TraceLevel level, IEnumerable<string> testIds, Stage stage, StageType type, object data = null, int methodsBack = 3)
        {
            LogInner_(level, testIds, stage, type, data, methodsBack);
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

        public void Log(TraceLevel level, Stage stage, StageType type, object data = null)
        {
            LogInner_(level, null, stage, type, data);
        }

        private void LogInner_(TraceLevel level, IEnumerable<string> testIds, Stage stage, StageType? type, object data,
            int methodsBack = 3)
        {
            if (skipLog_ || level < logHandler_.MinimumTraceLevel) return;
            string currentTime = DateTimeOffset.UtcNow.ToString(StandardDateTimeFormat.ISO8601);
            ClientEvent @event = new ClientEvent(currentTime, CreateMessageFromLog(testIds, stage, type, methodsBack, data), level);
            logHandler_.OnMessage(@event);
        }

        private Message CreateMessageFromLog(IEnumerable<string> testIds, Stage stage, StageType? type, int methodsBack,
            object data)
        {
            StackFrame stackFrame = new StackTrace(methodsBack, true).GetFrame(StackTrace.METHODS_TO_SKIP);
            MethodBase method = stackFrame.GetMethod();
            string stackTrace = GetStackTraceLine_(method);

            return new Message(AgentId, stage, type, testIds, Thread.CurrentThread.ManagedThreadId, stackTrace, data);
        }

        private string GetStackTraceLine_(MethodBase method)
        {
            if (!methodsCache_.TryGetValue(method, out string methodName))
            {
                string methodTypeName = GetTypeName(method.DeclaringType);
                string methodSignature = GetMethodSignature(method);
                methodName = $"{methodTypeName}.{methodSignature}";
                methodsCache_.TryAdd(method, methodName);
            }
            return methodName;
        }

        private string GetMethodSignature(MethodBase method)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append(method.Name);
            if (method.IsGenericMethod)
            {
                stringBuilder.Append('<');
                Type[] genArgs = method.GetGenericArguments();
                stringBuilder.Append(genArgs.Length);
                //foreach (Type genericArg in genArgs)
                //{
                //    stringBuilder.Append(GetTypeName(genericArg)).Append(", ");
                //}
                //if (genArgs.Length > 0)
                //{
                //    stringBuilder.Length -= 2;
                //}
                stringBuilder.Append('>');
            }
            stringBuilder.Append('(');

            ParameterInfo[] parameters = method.GetParameters();
            if (parameters.Length > 0) stringBuilder.Append(parameters.Length);
            //foreach (ParameterInfo pi in parameters)
            //{
            //    stringBuilder.Append(GetTypeName(pi.ParameterType)).Append(", ");
            //}
            //if (parameters.Length > 0)
            //{
            //    stringBuilder.Length -= 2;
            //}
            stringBuilder.Append(')');
            return stringBuilder.ToString();
        }

        public static string GetTypeName(Type type)
        {
            if (type == null)
                throw new ArgumentNullException(nameof(type));

            if (!type.IsGenericType)
                return type.Name;

            StringBuilder stringBuilder = new StringBuilder();
            BuildClassNameRecursive_(type, stringBuilder);
            return stringBuilder.ToString();
        }

        private static void BuildClassNameRecursive_(Type type, StringBuilder classNameBuilder, int genericParameterIndex = 0)
        {
            if (type.IsGenericParameter)
            {
                classNameBuilder.Append(GetTypeName(type));
            }
            else if (type.IsGenericType)
            {
                classNameBuilder.Append(GetTypeName_(type)).Append('<');
                classNameBuilder.Append(type.GetGenericArguments().Length);
                //int subIndex = 0;
                //foreach (Type genericTypeArgument in type.GetGenericArguments())
                //{
                //    if (subIndex > 0)
                //        classNameBuilder.Append(", ");

                //    BuildClassNameRecursive_(genericTypeArgument, classNameBuilder, subIndex++);
                //}
                classNameBuilder.Append('>');
            }
            else
                classNameBuilder.Append(GetTypeName_(type));
        }

        public static string GetNestedTypeName(Type type)
        {
            if (!type.IsNested)
                return type.Name;

            StringBuilder nestedName = new StringBuilder();
            while (type != null)
            {
                if (nestedName.Length > 0)
                    nestedName.Insert(0, '.');

                nestedName.Insert(0, GetTypeName_(type));

                type = type.DeclaringType;
            }
            return nestedName.ToString();
        }

        private static string GetTypeName_(Type type)
        {
            return type.Name.Split('`')[0];
        }
    }

}
