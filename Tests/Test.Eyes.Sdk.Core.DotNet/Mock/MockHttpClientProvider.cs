using Applitools.Utils;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools
{
    internal class MockHttpClientProvider : IHttpClientProvider
    {
        private readonly static HttpClient httpClient_;
        static MockHttpClientProvider()
        {
            HttpMessageHandler handler = new MockMessageProcessingHandler();
            httpClient_ = new HttpClient(handler);
        }

        public HttpClient GetClient()
        {
            return httpClient_;
        }

        class MockMessageProcessingHandler : HttpMessageHandler
        {
            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, 
                CancellationToken cancellationToken)
            {
                TaskCompletionSource<HttpResponseMessage> tcs = new TaskCompletionSource<HttpResponseMessage>();
                HttpResponseMessage response = new HttpResponseMessage(HttpStatusCode.OK);
                response.RequestMessage = request;
                tcs.SetResult(response);
                return tcs.Task;
            }
        }
    }
}