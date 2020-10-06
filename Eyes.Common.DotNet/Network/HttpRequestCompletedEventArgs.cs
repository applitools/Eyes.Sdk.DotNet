namespace Applitools.Utils
{
    using System;
    using System.Net;

    /// <summary>
    /// Http request completion event arguments.
    /// </summary>
    public class HttpRequestCompletedEventArgs : EventArgs
    {
        public HttpRequestCompletedEventArgs(
            TimeSpan elapsed, HttpWebRequest request, HttpWebResponse response)
        {
            ArgumentGuard.NotNull(request, nameof(request));
            ArgumentGuard.NotNull(response, nameof(response));

            Elapsed = elapsed;
            Request = request;
            Response = response;
        }

        /// <summary>
        /// The time is took the request to complete.
        /// </summary>
        public TimeSpan Elapsed { get; private set; }

        /// <summary>
        /// The HTTP request.
        /// </summary>
        public HttpWebRequest Request { get; private set; }

        /// <summary>
        /// The HTTP response.
        /// </summary>
        public HttpWebResponse Response { get; private set; }

        /// <inheritdoc />
        public override string ToString()
        {
            return "{0} {1} completed with status {2} in {3}ms".Fmt(
                Request.Method,
                Request.RequestUri,
                (int)Response.StatusCode,
                Elapsed.TotalMilliseconds);
        }
    }
}
