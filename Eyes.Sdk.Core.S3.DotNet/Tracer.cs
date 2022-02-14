using Applitools.Utils;
using System;
using System.Text;

namespace Applitools
{
    public class Tracer
    {
        /// <summary>
        /// Gets a method call trace message formatter.
        /// </summary>
        public static TracerMessageFormatter FormatCall(string methodName, params object[] args)
        {
            if (args == null)
            {
                args = new object[] { null };
            }

            return new TracerMessageFormatter_(methodName + "(", ")").WithArgs(args);
        }

        /// <summary>
        /// Gets the string representation of the input exception.
        /// </summary>
        public static string FormatException(Exception ex)
        {
            ArgumentGuard.NotNull(ex, nameof(ex));

            return "{0}: {1}\n{2}".Fmt(ex.GetType(), ex.Message, FormatExceptionStackTrace(ex));
        }

        /// <summary>
        /// Gets the stack trace string of the input exception and all its inner exceptions.
        /// </summary>
        public static string FormatExceptionStackTrace(Exception ex)
        {
            ArgumentGuard.NotNull(ex, nameof(ex));

            var sb = new StringBuilder();
            GetStackTraceString_(sb, ex);
            return sb.ToString();
        }

        private static void GetStackTraceString_(StringBuilder sb, Exception ex)
        {
            sb.AppendLine(ex.StackTrace);
            if (ex.InnerException != null)
            {
                sb.AppendLine("--- INNER EXCEPTION: {0} - {1} ---"
                    .Fmt(ex.InnerException.GetType(), ex.InnerException.Message));
                GetStackTraceString_(sb, ex.InnerException);
            }
        }

        private class TracerMessageFormatter_ : TracerMessageFormatter
        {
            public TracerMessageFormatter_(string prefix, string suffix = "")
                : base(prefix, suffix)
            {
            }
        }
    }
}