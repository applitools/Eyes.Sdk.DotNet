using System.Net;
using System.Net.Http;

namespace Applitools.Utils
{
    internal class HttpClientProvider : IHttpClientProvider
    {
        private readonly static HttpClient httpClient_;
        static HttpClientProvider()
        {
            HttpClientHandler handler = new HttpClientHandler()
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate
            };
            httpClient_ = new HttpClient(handler);
            Instance = new HttpClientProvider();
        }

        public static IHttpClientProvider Instance { get; }

        public HttpClient GetClient()
        {
            return httpClient_;
        }
    }
}