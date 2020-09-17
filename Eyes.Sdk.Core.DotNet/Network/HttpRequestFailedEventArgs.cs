namespace Applitools.Utils
{
    using System;
    using System.Net;

    /// <summary>
    /// Http request failed (with exception) event arguments.
    /// </summary>
    public class HttpRequestFailedEventArgs : EventArgs
    {
        public HttpRequestFailedEventArgs(
            TimeSpan elapsed, HttpWebRequest request, Exception exception)
        {
            ArgumentGuard.NotNull(request, nameof(request));
            ArgumentGuard.NotNull(exception, nameof(exception));

            Elapsed = elapsed;
            Request = request;
            Exception = exception;
        }

        /// <summary>
        /// The time that elapsed until the failure was reported.
        /// </summary>
        public TimeSpan Elapsed { get; private set; }

        /// <summary>
        /// The HTTP request.
        /// </summary>
        public HttpWebRequest Request { get; private set; }

        /// <summary>
        /// The exception thrown.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return "{0} {1} failed after {2}ms:\n{3}".Fmt(
                Request.Method,
                Request.RequestUri,
                Elapsed.TotalMilliseconds,
                Tracer.FormatException(Exception));
        }
    }
}
