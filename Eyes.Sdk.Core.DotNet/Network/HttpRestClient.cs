using System;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using Newtonsoft.Json;
using System.Diagnostics;
using System.Collections.Specialized;

namespace Applitools.Utils
{
    /// <summary>
    /// An HTTP rest client.
    /// </summary>
    /// <remarks>Make sure to <c>Dispose</c> or <c>Close</c> responses!</remarks>
    public class HttpRestClient
    {
        #region Fields

        private readonly JsonSerializer json_;
        private string authUser_;
        private string authPassword_;

        #endregion

        #region Constructors

        static HttpRestClient()
        {
            ServicePointManager.ServerCertificateValidationCallback +=
                new RemoteCertificateValidationCallback(ValidateRemoteCertificate_);
        }

        /// <summary>
        /// Creates a new <see cref="HttpRestClient"/> instance.
        /// </summary>
        /// <param name="jsonSerializer">The JSON serializer to use to <c>null</c> 
        /// to use the default serializer.</param>
        /// <param name="agentId">The full agent ID of the SDK.</param>
        /// <param name="serverUrl">Server's base URL</param>
        public HttpRestClient(Uri serverUrl, string agentId = null, JsonSerializer jsonSerializer = null)
        {
            ArgumentGuard.NotNull(serverUrl, nameof(serverUrl));

            ServerUrl = serverUrl;
            json_ = jsonSerializer ?? JsonUtils.CreateSerializer(false, false);
            AgentId = agentId;
            ConnectionLimit = 10;
            Timeout = TimeSpan.FromMinutes(10);
        }

        private HttpRestClient(HttpRestClient other)
        {
            ServerUrl = other.ServerUrl;
            json_ = other.json_;
            AgentId = other.AgentId;
            ConnectionLimit = other.ConnectionLimit;
            Timeout = other.Timeout;
            Proxy = other.Proxy;
            Headers = new NameValueCollection(other.Headers);
            authUser_ = other.authUser_;
            authPassword_ = other.authPassword_;
            Retry = other.Retry;
            FormatRequestUri = other.FormatRequestUri;
            ConfigureRequest = other.ConfigureRequest;
            RequestCompleted = other.RequestCompleted;
            RequestFailed = other.RequestFailed;
            WebRequestCreator = other.WebRequestCreator;
        }
        #endregion

        #region Events

        /// <summary>
        /// Fired before each request is sent.
        /// </summary>
        public event EventHandler<HttpWebRequestEventArgs> ConfigureRequest;

        /// <summary>
        /// Fired after a request completes and a response is available.
        /// </summary>
        public event EventHandler<HttpRequestCompletedEventArgs> RequestCompleted;

        /// <summary>
        /// Fired after a request has failed (due to an exception).
        /// </summary>
        public event EventHandler<HttpRequestFailedEventArgs> RequestFailed;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the server's base url.
        /// </summary>
        public Uri ServerUrl { get; set; }

        /// <summary>
        /// Gets or sets the HTTP request timeout.
        /// </summary>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Determines whether the input request should be retied based on its response.
        /// </summary>
        public Func<HttpWebRequest, HttpWebResponse, bool> Retry { get; set; }

        /// <summary>
        /// Determines the maximum number of concurrent connections (default is 10).
        /// Excess connections will block.
        /// </summary>
        public int ConnectionLimit { get; set; }

        /// <summary>
        /// Formats HTTP request URIs.
        /// </summary>
        /// <returns>A new URI to use in the request</returns>
        public Func<Uri, Uri> FormatRequestUri { get; set; }

        /// <summary>
        /// Gets or sets the proxy used by this client or <c>null</c> to obtain the system 
        /// proxy.
        /// </summary>
        public WebProxy Proxy { get; set; }

        public NameValueCollection Headers { get; } = new NameValueCollection();
        public string AgentId { get; set; }
        internal IWebRequestCreate WebRequestCreator { get; set; } = DefaultWebRequestCreator.Instance;
        #endregion

        #region Methods

        #region Public

        /// <summary>
        /// Sets basic authentication credentials.
        /// </summary>
        public void SetBasicAuth(string userName, string password)
        {
            authUser_ = userName;
            authPassword_ = password;
        }

        /// <summary>
        /// Sends a POST request to the input path under the base url.
        /// </summary>
        public HttpWebResponse Post(string path, string accept = null)
        {
            return SendHttpWebRequest_(path, "POST", null, null, accept, null);
        }

        /// <summary>
        /// Sends a POST request to the input path under the base url with a JSON body.
        /// </summary>
        public HttpWebResponse PostJson<T>(string path, T body, string accept = "application/json")
        {
            return SendJsonHttpWebRequest_(path, "POST", body, accept);
        }

        /// <summary>
        /// Sends a POST request to the input path under the base url with a JSON body.
        /// </summary>
        public void PostJson<T>(TaskListener<HttpWebResponse> listener, string path, T body, string accept = "application/json")
        {
            SendLongJsonRequest_(listener, path, "POST", body, accept);
        }

        /// <summary>
        /// Sends a POST request of the input body to the input path under the base url.
        /// </summary>
        public HttpWebResponse Post(
            string path, MemoryStream body, string contentType = null, string accept = null, string contentEncoding = null)
        {
            return SendHttpWebRequest_(path, "POST", body, contentType, accept, contentEncoding);
        }

        /// <summary>
        /// Sends a PUT request to the input path under the base url.
        /// </summary>
        public HttpWebResponse Put(string path, string accept = null)
        {
            return SendHttpWebRequest_(path, "PUT", null, null, accept, null);
        }

        /// <summary>
        /// Sends a PUT request to the input path under the base url with a JSON body.
        /// </summary>
        public HttpWebResponse PutJson<T>(string path, T body, string accept = "application/json")
        {
            return SendJsonHttpWebRequest_(path, "PUT", body, accept);
        }

        /// <summary>
        /// Sends a PUT request of the input body to the input path under the base url.
        /// </summary>
        public HttpWebResponse Put(
            string path, MemoryStream body, string contentType = null, string accept = null, string contentEncoding = null)
        {
            return SendHttpWebRequest_(path, "PUT", body, contentType, accept, contentEncoding);
        }

        /// <summary>
        /// Sends a DELETE request to the input path under the base url.
        /// </summary>
        public HttpWebResponse Delete(string path, string accept = null)
        {
            return SendHttpWebRequest_(path, "DELETE", null, null, accept, null);
        }

        /// <summary>
        /// Sends a DELETE request to the input path under the base url.
        /// </summary>
        public void Delete(TaskListener<HttpWebResponse> listener, string path, string accept = null)
        {
            SendLongRequest_(listener, path, "DELETE", null, null, accept, null);
        }

        /// <summary>
        /// Sends a DELETE request to the input path under the base url with a Json body.
        /// </summary>
        public void DeleteJson<T>(TaskListener<HttpWebResponse> listener,
            string path, T body, string accept = "application/json")
        {
            SendLongJsonRequest_(listener, path, "DELETE", body, accept);
        }

        /// <summary>
        /// Sends a GET request to the input path under the base url.
        /// </summary>
        public HttpWebResponse Get(string path, string accept = null)
        {
            return SendHttpWebRequest_(path, "GET", null, null, accept, null);
        }

        /// <summary>
        /// Sends a GET request accepting a Json response to the input path under the base url.
        /// </summary>
        public HttpWebResponse GetJson(string path)
        {
            return Get(path, "application/json");
        }

        protected virtual void ConfigureHttpWebRequest(HttpWebRequest request)
        {
        }

        public HttpRestClient Clone()
        {
            return new HttpRestClient(this);
        }

        #endregion

        #region Private

        private static bool ValidateRemoteCertificate_(
            object sender, X509Certificate cert, X509Chain chain, SslPolicyErrors policyErrors)
        {
            return true;
        }

        private HttpWebResponse SendJsonHttpWebRequest_<T>(
            string path, string method, T body, string accept)
        {
            using (var s = new MemoryStream())
            {
                json_.Serialize(body, s);
                s.Position = 0;

                return SendHttpWebRequest_(path, method, s, "application/json", accept, null);
            }
        }

        private void SendLongJsonRequest_<T>(
            TaskListener<HttpWebResponse> listener,
            string path, string method, T body, string accept)
        {
            using (var s = new MemoryStream())
            {
                json_.Serialize(body, s);
                s.Position = 0;

                SendLongRequest_(listener, path, method, s, "application/json", accept, null);
            }
        }

        public void SendAsyncRequest(TaskListener<HttpWebResponse> listener, string url, string method)
        {
            Uri requestUri = new Uri(url);
            HttpWebRequest request = CreateHttpWebRequest_(requestUri, method, null, null, null, null);
            SendAsyncRequest(listener, request);
        }

        public static void SendAsyncRequest(TaskListener<HttpWebResponse> listener, HttpWebRequest request)
        {
            request.BeginGetResponse(ar =>
            {
                if (!ar.IsCompleted) return;
                HttpWebRequest resultRequest = (HttpWebRequest)ar.AsyncState;
                try
                {
                    HttpWebResponse response = (HttpWebResponse)resultRequest.EndGetResponse(ar);
                    listener.OnComplete(response);
                    response.Close();
                }
                catch (WebException ex)
                {
                    listener.OnFail(ex);
                }
            }, request);
        }

        private HttpWebResponse SendHttpWebRequest_(
            string path, string method, MemoryStream body, string contentType, string accept, string contentEncoding)
        {
            Uri requestUri = string.IsNullOrEmpty(path) ? ServerUrl : new Uri(ServerUrl, path);

            HttpWebRequest request = CreateHttpWebRequest_(
               requestUri, method, body, contentType, accept, contentEncoding);

            if (request == null)
            {
                throw new NullReferenceException("request is null");
            }

            return (HttpWebResponse)request.GetResponse();
        }

        private void SendLongRequest_(TaskListener<HttpWebResponse> listener,
            string path, string method, MemoryStream body, string contentType, string accept, string contentEncoding)
        {
            HttpWebRequest request = null;
            HttpWebResponse response = null;

            Stopwatch sw = Stopwatch.StartNew();

            try
            {
                try
                {
                    var requestUri = string.IsNullOrEmpty(path) ?
                        ServerUrl : new Uri(ServerUrl, path);

                    request = CreateHttpWebRequest_(
                       requestUri, method, body, contentType, accept, contentEncoding);

                    if (request == null)
                    {
                        throw new NullReferenceException("request is null");
                    }
                    request.BeginGetResponse(OnLongRequestResponse_, request);
                }
                catch (WebException ex)
                {
                    if (request == null || ex.Response == null)
                    {
                        throw;
                    }

                    listener.OnFail(ex);
                }
            }
            catch (Exception ex2)
            {
                if (request != null && RequestFailed != null)
                {
                    var args = new HttpRequestFailedEventArgs(sw.Elapsed, request, ex2);
                    CommonUtils.DontThrow(() => RequestFailed(this, args));
                }

                throw;
            }

            void OnLongRequestResponse_(IAsyncResult result)
            {
                if (!result.IsCompleted) return;
                HttpWebRequest resultRequest = (HttpWebRequest)result.AsyncState;
                response = (HttpWebResponse)resultRequest.EndGetResponse(result);

                if (response == null)
                {
                    throw new NullReferenceException("response is null");
                }

                string statusUrl = response.Headers[HttpResponseHeader.Location];
                RequestPollingTaskListener requestPollingListener = new RequestPollingTaskListener(this, statusUrl, listener);
                SendAsyncRequest(requestPollingListener, statusUrl, "GET");
            }

        }

        private HttpWebRequest CreateHttpWebRequest_(
            Uri requestUri, string method, MemoryStream body, string contentType, string accept, string contentEncoding)
        {
            if (FormatRequestUri != null)
            {
                requestUri = FormatRequestUri(requestUri);
            }

            //var req = (HttpWebRequest)WebRequest.Create(requestUri);
            var req = (HttpWebRequest)WebRequestCreator.Create(requestUri);

            if (Proxy != null)
            {
                req.Proxy = Proxy;
            }
            else
            {
#if NET45
                // Apply system web proxy configuration.
                var proxy = WebRequest.GetSystemWebProxy();
                if (proxy != null)
                {
                    var proxyUri = proxy.GetProxy(req.RequestUri).ToString();
                    if (proxyUri != requestUri.ToString())
                    {
                        req.Proxy = new WebProxy(proxyUri, false);
                        req.Proxy.Credentials = CredentialCache.DefaultCredentials;
                    }
                }
#endif
            }

#if NET45
            req.ServicePoint.ConnectionLimit = ConnectionLimit;
#endif

            if (accept != null)
            {
                req.Accept = accept;
            }

            req.Method = method;
            if (authUser_ != null || authPassword_ != null)
            {
                req.BasicAuthentication(authUser_, authPassword_);
            }

            if (Timeout != TimeSpan.Zero)
            {
                req.Timeout = (int)Timeout.TotalMilliseconds;
            }

            if (ConfigureRequest != null)
            {
                var args = new HttpWebRequestEventArgs(req);
                CommonUtils.DontThrow(() => ConfigureRequest(this, args));
            }

            if (Headers.Count > 0)
            {
                req.Headers.Add(Headers);
            }

            ConfigureHttpWebRequest(req);

            if (AgentId != null)
            {
                req.Headers["x-applitools-eyes-client"] = AgentId;
            }

            req.Headers["x-applitools-eyes-client-request-id"] = Guid.NewGuid().ToString();

            SetLongRequestHeaders(req);

            if (body != null)
            {
                if (contentType != null)
                {
                    req.ContentType = contentType;
                }

                if (contentEncoding != null)
                {
                    req.Headers["Content-Encoding"] = contentEncoding;
                }

                req.ContentLength = body.Length;
                body.Position = 0;
                body.CopyTo(req.GetRequestStream());
            }

            return req;
        }

        private static void SetLongRequestHeaders(HttpWebRequest req)
        {
            req.Headers["Eyes-Expect"] = "202+location";
            req.Headers["Eyes-Expect-Version"] = "2";
            req.Headers["Eyes-Date"] =
                TimeUtils.ToString(DateTimeOffset.Now, StandardDateTimeFormat.RFC1123);
        }

        #endregion

        #endregion
    }

    internal class DefaultWebRequestCreator : IWebRequestCreate
    {
        private DefaultWebRequestCreator() { }
        public static DefaultWebRequestCreator Instance = new DefaultWebRequestCreator();
        public WebRequest Create(Uri uri)
        {
            HttpWebRequest request = WebRequest.CreateHttp(uri);
            request.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            return request;
        }
    }
}
