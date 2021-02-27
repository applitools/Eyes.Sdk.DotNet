using Applitools.Utils;
using Newtonsoft.Json;
using NSubstitute;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;

namespace Applitools
{
    class MockServerConnector : ServerConnector
    {
        public MockServerConnector(Logger logger)
            : this(logger, new Uri("http://some.url.com"))
        {
        }

        public MockServerConnector(Logger logger, Uri serverUrl)
            : base(logger, serverUrl)
        {
            logger.Verbose("created");
            HttpRestClientFactory = new MockHttpRestClientFactory(Logger);
        }

        public List<Uri> CloseBatchRequestUris { get; } = new List<Uri>();

        protected override HttpResponseMessage CloseBatchImpl_(string batchId, HttpRestClient httpClient)
        {
            HttpResponseMessage response = base.CloseBatchImpl_(batchId, httpClient);
            CloseBatchRequestUris.Add(response.RequestMessage.RequestUri);
            return response;
        }
    }

    class MockHttpRestClientFactory : IHttpRestClientFactory
    {
        public MockHttpRestClientFactory(Logger logger)
        {
            Logger = logger;
        }

        public Logger Logger { get; set; }

        public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
        {
            IHttpClientProvider mockProvider = new MockHttpClientProvider();
            HttpRestClient mockedHttpRestClient = new HttpRestClient(serverUrl, agentId, jsonSerializer, Logger, mockProvider);
            return mockedHttpRestClient;
        }
    }

    class MockWebRequestCreator : IWebRequestCreate
    {
        public MockWebRequestCreator(Logger logger)
        {
            Logger = logger;
        }

        public Logger Logger { get; }

        public WebRequest Create(Uri uri)
        {
            Logger?.Verbose("creating mock request for URI: {0}", uri);
            HttpWebRequest webRequest = Substitute.For<HttpWebRequest>();
            webRequest.RequestUri.Returns(uri);
            webRequest.Headers = new WebHeaderCollection();
            HttpWebResponse response = Substitute.For<HttpWebResponse>();
            response.StatusCode.Returns(HttpStatusCode.OK);
            response.ResponseUri.Returns(uri);
            webRequest.GetResponse().Returns(response);
            return webRequest;
        }
    }
}