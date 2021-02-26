namespace Applitools.Utils
{
    using Newtonsoft.Json;
    using System;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Text;
    using System.Web;

    /// <summary>
    /// Network related utilities.
    /// </summary>
    public static class NetworkUtils
    {
        #region Methods

        /// <summary>
        /// Adds a basic authentication <c>Authorization</c> header to the specified web request.
        /// </summary>
        public static HttpWebRequest BasicAuthentication(this HttpWebRequest request, string userName, string password)
        {
            ArgumentGuard.NotNull(request, nameof(request));

            userName = userName ?? string.Empty;
            password = password ?? string.Empty;

            var token = Convert.ToBase64String(UTF8Encoding.UTF8.GetBytes(
                "{0}:{1}".Fmt(userName, password)));

            request.Headers["Authorization"] = "Basic " + token;
            return request;
        }

        #region URLs

        /// <summary>
        /// URL encodes value while working around issues with <see cref="HttpUtility.UrlEncode(String)"/>.
        /// </summary>
        /// <param name="value">The string to encode.</param>
        /// <returns>The encoded string.</returns>
        public static string UrlEncode(string value)
        {
            var encoded = HttpUtility.UrlEncode(value);
            if (encoded != null)
            {
                encoded = encoded.Replace("+", "%20");
            }

            return encoded;
        }

        /// <summary>
        /// Encodes and adds the input query argument to the input URI and returns the resulting 
        /// URI.
        /// </summary>
        public static Uri AddUriQueryArg(this Uri uri, string name, string value)
        {
            ArgumentGuard.NotNull(uri, nameof(uri));
            ArgumentGuard.TrimNotEmpty(ref name, nameof(name));
            ArgumentGuard.TrimNotNull(ref value, nameof(value));

            var fragment = "{0}={1}".Fmt(name, UrlEncode(value));
            var query = uri.Query.Trim();
            if (query.Length == 0)
            {
                query = "?" + fragment;
            }
            else
            {
                query = query + "&" + fragment;
            }

            return new Uri(uri.GetLeftPart(UriPartial.Path) + query);
        }

        #endregion

        #region Json

        /// <summary>
        /// Deserializes the body of the response if its status code is <c>200 OK</c>.
        /// </summary>
        /// <param name="dispose">Whether or not to dispose of the response</param>
        /// <param name="response">Response which body to deserialize</param>
        public static T DeserializeBody<T>(this HttpWebResponse response, bool dispose)
        {
            return DeserializeBody<T>(response, dispose, null, new int[0]);
        }

        /// <summary>
        /// Deserializes the body of the response if its status code is <c>200 OK</c> or any 
        /// of the specified status codes.
        /// </summary>
        /// <param name="dispose">Whether or not to dispose of the response</param>
        /// <param name="response">Response which body to deserialize</param>
        /// <param name="serializer">Json serializer to use</param>
        /// <param name="validStatuses">Valid status codes</param>
        public static T DeserializeBody<T>(
            this HttpWebResponse response,
            bool dispose,
            JsonSerializer serializer,
            params HttpStatusCode[] validStatuses)
        {
            var codes = new int[validStatuses.Length];
            for (int i = 0; i < codes.Length; ++i)
            {
                codes[i] = (int)validStatuses[i];
            }

            return DeserializeBody<T>(response, dispose, serializer, codes);
        }

        /// <summary>
        /// Deserializes the body of the response if its status code is <c>200 OK</c> or any 
        /// of the specified status codes.
        /// </summary>
        /// <param name="dispose">Whether or not to dispose of the response</param>
        /// <param name="response">Response which body to deserialize</param>
        /// <param name="serializer">Json serializer to use</param>
        /// <param name="validStatuses">Valid status codes</param>
        public static T DeserializeBody<T>(
            this HttpWebResponse response,
            bool dispose,
            JsonSerializer serializer,
            params int[] validStatuses)
        {
            ArgumentGuard.NotNull(response, nameof(response));

            serializer = serializer ?? JsonUtils.Serializer;

            if (response.StatusCode != HttpStatusCode.OK &&
                !validStatuses.Contains((int)response.StatusCode))
            {
                var msg = "Unexpected status: {0} {1}"
                    .Fmt((int)response.StatusCode, response.StatusDescription);
                throw new WebException(msg, null, WebExceptionStatus.ProtocolError, response);
            }

            using (var s = response.GetResponseStream())
            {
                var json = new StreamReader(s).ReadToEnd();

                try
                {
                    var body = serializer.Deserialize<T>(json);
                    if (dispose)
                    {
                        response.Close();
                    }

                    return body;
                }
                catch (Exception ex)
                {
                    throw new FormatException("Failed to deserialize '{0}'".Fmt(json), ex);
                }
            }
        }

        public static T DeserializeBody<T>(
            this HttpResponseMessage response,
            bool dispose,
            JsonSerializer serializer,
            params HttpStatusCode[] validStatuses)
        {
            var codes = new int[validStatuses.Length];
            for (int i = 0; i < codes.Length; ++i)
            {
                codes[i] = (int)validStatuses[i];
            }

            return DeserializeBody<T>(response, dispose, serializer, codes);
        }

        public static T DeserializeBody<T>(this HttpResponseMessage response, bool dispose)
        {
            return DeserializeBody<T>(response, dispose, null, new int[0]);
        }

        public static T DeserializeBody<T>(
           this HttpResponseMessage response,
           bool dispose,
           JsonSerializer serializer,
           params int[] validStatuses)
        {
            ArgumentGuard.NotNull(response, nameof(response));

            serializer = serializer ?? JsonUtils.Serializer;

            if (response.StatusCode != HttpStatusCode.OK &&
                !validStatuses.Contains((int)response.StatusCode))
            {
                var msg = "Unexpected status: {0} {1}"
                    .Fmt((int)response.StatusCode, response.ReasonPhrase);
                throw new WebException(msg, WebExceptionStatus.ProtocolError);
            }

            using (var s = response.Content.ReadAsStreamAsync().Result)
            {
                var json = new StreamReader(s).ReadToEnd();

                try
                {
                    var body = serializer.Deserialize<T>(json);
                    if (dispose)
                    {
                        response.Dispose();
                    }

                    return body;
                }
                catch (Exception ex)
                {
                    throw new FormatException("Failed to deserialize '{0}'".Fmt(json), ex);
                }
            }
        }

        public static Stream GetRequestStream(this HttpRequestMessage request)
        {
            return request.Content.ReadAsStreamAsync().Result;
        }
        #endregion

        #endregion
    }
}
