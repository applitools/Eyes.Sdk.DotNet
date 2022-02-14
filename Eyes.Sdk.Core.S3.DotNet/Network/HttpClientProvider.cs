using System.Net;
using System.Net.Http;

namespace Applitools.Utils
{
    internal class HttpClientProvider : IHttpClientProvider
    {
        private static HttpClient httpClient_;
        private static IWebProxy proxy_;

        static HttpClientProvider()
        {
            Instance = new HttpClientProvider();
        }

        private static void CreateHttpClient()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            if (proxy_ != null)
            {
                handler.Proxy = proxy_;
                handler.UseProxy = true;
            }
            httpClient_ = new HttpClient(handler);
        }

        public static IHttpClientProvider Instance { get; }

        public HttpClient GetClient(IWebProxy proxy)
        {
            if (httpClient_ == null || proxy_ != proxy)
            {
                proxy_ = proxy;
                CreateHttpClient();
            }
            return httpClient_;
        }
    }
}