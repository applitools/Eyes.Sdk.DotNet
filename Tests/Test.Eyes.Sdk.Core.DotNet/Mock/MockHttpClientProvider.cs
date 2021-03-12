using Applitools.Utils;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools
{
    internal class MockHttpClientProvider : IHttpClientProvider
    {
        private readonly HttpClient httpClient_;
        public MockHttpClientProvider()
        {
            HttpMessageHandler handler = new MockMessageProcessingHandler();
            httpClient_ = new HttpClient(handler);
        }

        public HttpClient GetClient(IWebProxy proxy)
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