using NUnit.Framework;
using System.Net;
using System;
using System.Drawing;
using Applitools.Utils;
using Newtonsoft.Json;
using NSubstitute;
using System.IO;
using System.Text;
using Applitools.Tests.Utils;
using System.Collections.Generic;
using System.Diagnostics;

namespace Applitools
{
    [TestFixture]
    public class TestRunningSession : ReportingTestSuite
    {
        private static readonly string BASE_LOCATION = "api/tasks/123412341234/";

        [Test]
        public void TestDataStructure()
        {
            RunningSession rs = new RunningSession();
            Assert.IsNull(rs.isNewSession_);
            Assert.IsNull(rs.Id);
            Assert.IsNull(rs.Url);
            rs.isNewSession_ = true;
            rs.Url = "dummy url";
            rs.Id = "dummy id";
            Assert.AreEqual(true, rs.IsNewSession);
            Assert.AreEqual("dummy url", rs.Url);
            Assert.AreEqual("dummy id", rs.Id);
        }

        [TestCase(true)]
        [TestCase(false)]
        public void TestStartSession_LongRequest_IsNewField(bool isNew)
        {
            Logger logger = new Logger();
            ServerConnector serverConnector = new ServerConnector(logger);
            serverConnector.ServerUrl = new Uri(CommonData.DefaultServerUrl);
            serverConnector.HttpRestClientFactory = new MockHttpRestClientFactory(isNew: isNew);
            serverConnector.ApiKey = "testKey";
            SessionStartInfo startInfo = GetStartInfo();
            RunningSession result = serverConnector.StartSession(startInfo);
            Assert.AreEqual(isNew, result.IsNewSession);
        }

        private SessionStartInfo GetStartInfo()
        {
            PropertiesCollection sessionProperties = new PropertiesCollection
            {
                { "property 1", "value 1" },
                { null, null }
            };

            BatchInfo batchInfo = new BatchInfo("some batch", new DateTimeOffset(new DateTime(2017, 6, 29, 11, 1, 0, DateTimeKind.Utc)));
            batchInfo.Id = "someBatchId";

            SessionStartInfo sessionStartInfo = new SessionStartInfo(
                "agent",
                "some app",
                "1.0",
                "some test",
                batchInfo,
                "baseline",
                new AppEnvironment("windows", "test suite", new Size(234, 456)) { DeviceInfo = "Some Mobile Device" },
                "some environment",
                new ImageMatchSettings(MatchLevel.Strict),
                "some branch",
                "parent branch",
                "baseline branch",
                saveDiffs: null,
                render: null,
                agentSessionId: "59436361-2782-45EF-9DC5-5633F15150CE",
                timeout: null,
                properties: sessionProperties);

            return sessionStartInfo;
        }

        [TestCase(HttpStatusCode.Created)]
        [TestCase(HttpStatusCode.OK)]
        public void TestStartSession_LongRequest_StatusCode(HttpStatusCode statusCode)
        {
            Logger logger = new Logger();
            ServerConnector serverConnector = new ServerConnector(logger);
            serverConnector.ServerUrl = new Uri(CommonData.DefaultServerUrl);
            serverConnector.HttpRestClientFactory = new MockHttpRestClientFactory(statusCode: statusCode);
            serverConnector.ApiKey = "testKey";
            SessionStartInfo startInfo = GetStartInfo();
            RunningSession result = serverConnector.StartSession(startInfo);

            bool isNew = statusCode == HttpStatusCode.Created;
            Assert.AreEqual(isNew, result.IsNewSession);
        }

        [Test]
        public void TestLongRequest_SimplePoll()
        {
            Logger logger = new Logger();
            ServerConnector serverConnector = new ServerConnector(logger);
            serverConnector.ServerUrl = new Uri(CommonData.DefaultServerUrl);
            serverConnector.HttpRestClientFactory = new MockHttpRestClientFactory(
                new int?[] { 2, null, 3 },
                new string[] { CommonData.DefaultServerUrl + "url1", null, CommonData.DefaultServerUrl + "url2" });
            serverConnector.ApiKey = "testKey";

            SessionStartInfo startInfo = GetStartInfo();
            RunningSession result = serverConnector.StartSession(startInfo);

            MockHttpRestClientFactory.MockWebRequestCreator requestCreator = (MockHttpRestClientFactory.MockWebRequestCreator)serverConnector.httpClient_.WebRequestCreator;
            List<string> requests = requestCreator.RequestUrls;
            List<TimeSpan> timings = requestCreator.Timings;
            Assert.AreEqual(6, requests.Count);
            Assert.AreEqual(6, timings.Count);
            StringAssert.StartsWith(CommonData.DefaultServerUrl + "api/sessions/running", requests[0]);
            StringAssert.StartsWith(CommonData.DefaultServerUrl + BASE_LOCATION + "status", requests[1]);

            StringAssert.StartsWith(CommonData.DefaultServerUrl + "url1", requests[2]);
            Assert.Greater(timings[2], TimeSpan.FromSeconds(2));

            StringAssert.StartsWith(CommonData.DefaultServerUrl + "url1", requests[3]);
            Assert.Greater(timings[3], TimeSpan.FromSeconds(0.5));

            StringAssert.StartsWith(CommonData.DefaultServerUrl + "url2", requests[4]);
            Assert.Greater(timings[4], TimeSpan.FromSeconds(3));
        }

        private class MockHttpRestClientFactory : IHttpRestClientFactory
        {
            private bool? isNew_;
            private HttpStatusCode? statusCode_;
            private string[] pollingUrls_;
            private int?[] retryAfter_;

            public MockHttpRestClientFactory(int?[] retryAfter, string[] pollingUrls)
            {
                if (retryAfter != null && pollingUrls != null && retryAfter.Length != pollingUrls.Length)
                {
                    throw new Exception("lists must be the same size");
                }
                retryAfter_ = retryAfter;
                pollingUrls_ = pollingUrls;
            }

            public MockHttpRestClientFactory(bool? isNew = null, HttpStatusCode? statusCode = null)
            {
                isNew_ = isNew;
                statusCode_ = statusCode;
            }

            public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
            {
                var client = new HttpRestClient(serverUrl, agentId, jsonSerializer);
                client.WebRequestCreator = new MockWebRequestCreator(isNew_, statusCode_, retryAfter_, pollingUrls_);
                return client;
            }

            internal class MockWebRequestCreator : IWebRequestCreate
            {
                private Stream responseStream_;
                private int counter_ = 0;
                private HttpStatusCode? statusCode_;
                private string[] pollingUrls_;
                private int?[] retryAfter_;
                private int iterations_;
                private string expectedPollUrlPath_;
                private Stopwatch stopwatch_ = Stopwatch.StartNew();
                public List<string> RequestUrls { get; } = new List<string>();
                public List<TimeSpan> Timings { get; } = new List<TimeSpan>();

                public MockWebRequestCreator(bool? isNew, HttpStatusCode? statusCode, int?[] retryAfter, string[] pollingUrls)
                {
                    RunningSession runningSession = new RunningSession()
                    {
                        Id = "MDAwMDAyNTE4MTU1OTk5NDE2NjQ~;MDAwMDAyNTE4MTU1OTk5NDExNDE~",
                        SessionId = "00000251815599941141",
                        BatchId = "00000251815599941664",
                        BaselineId = "16c7e248-7732-4ed3-b380-47889492ccc4.5d9e472a-8b3b-4e3d-ae86-72efcf77e19e.l_j7LZkpj6_rUc9bgnXRbyyZPvlLi00PX_7ZsJbd7ls_.",
                        isNewSession_ = isNew,
                        Url = "https://eyes.applitools.com/app/batches/00000251815599941664/00000251815599941141?accountId=m9QzkQCbDkyTxwMrJ4fKkQ~~"
                    };
                    statusCode_ = statusCode;
                    pollingUrls_ = pollingUrls;
                    retryAfter_ = retryAfter;
                    iterations_ = pollingUrls?.Length ?? retryAfter?.Length ?? 0;
                    string responseObjJson = JsonConvert.SerializeObject(runningSession);
                    byte[] byteArray = Encoding.UTF8.GetBytes(responseObjJson);
                    responseStream_ = new MemoryStream(byteArray);
                }

                public WebRequest Create(Uri url)
                {
                    var webRequest = Substitute.For<HttpWebRequest>();
                    webRequest.Headers.Returns(new WebHeaderCollection());
                    webRequest.GetRequestStream().Returns(new MemoryStream(new byte[2000]));
                    webRequest.RequestUri.Returns(url);
                    webRequest.BeginGetResponse(default, default)
                        .ReturnsForAnyArgs(ci =>
                        {
                            AsyncCallback cb = ci.Arg<AsyncCallback>();
                            HttpWebRequest req = ci.Arg<HttpWebRequest>();
                            cb.Invoke(new MockAsyncResult(req));
                            return null;
                        });

                    webRequest.EndGetResponse(default)
                        .ReturnsForAnyArgs(ci =>
                        {
                            MockAsyncResult mockAsyncResult = ci.Arg<MockAsyncResult>();
                            Uri uri = ((HttpWebRequest)mockAsyncResult.AsyncState).RequestUri;
                            var webResponse = Substitute.For<HttpWebResponse>();
                            WebHeaderCollection headers = new WebHeaderCollection();
                            webResponse.Headers.Returns(headers);
                            if (statusCode_ == null)
                            {
                                if (uri.PathAndQuery.StartsWith("/api/sessions/running", StringComparison.OrdinalIgnoreCase))
                                {
                                    webResponse.StatusCode.Returns(HttpStatusCode.Accepted);
                                    expectedPollUrlPath_ = "/" + BASE_LOCATION + "status";
                                    headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "status");
                                }
                                else if (uri.PathAndQuery.StartsWith(expectedPollUrlPath_, StringComparison.OrdinalIgnoreCase))
                                {
                                    if (iterations_ == 0 && counter_ == 0)
                                    {
                                        webResponse.StatusCode.Returns(HttpStatusCode.OK);
                                    }
                                    else if (iterations_ > 0 && counter_ < iterations_)
                                    {
                                        webResponse.StatusCode.Returns(HttpStatusCode.OK);

                                        if (pollingUrls_ != null)
                                        {
                                            string pollUrl = pollingUrls_[counter_];
                                            if (pollUrl != null)
                                            {
                                                headers.Add(HttpResponseHeader.Location, pollUrl);
                                                expectedPollUrlPath_ = new Uri(pollUrl).PathAndQuery;
                                            }
                                        }

                                        if (retryAfter_ != null)
                                        {
                                            int? retryAfter = retryAfter_[counter_];
                                            if (retryAfter != null) headers.Add(HttpResponseHeader.RetryAfter, retryAfter.Value.ToString());
                                        }
                                    }
                                    else
                                    {
                                        webResponse.StatusCode.Returns(HttpStatusCode.Created);
                                        headers.Add(HttpResponseHeader.Location, CommonData.DefaultServerUrl + BASE_LOCATION + "result");
                                    }
                                    counter_++;
                                }
                                else if (uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "result", StringComparison.OrdinalIgnoreCase))
                                {
                                    webResponse.StatusCode.Returns(HttpStatusCode.OK);
                                    webResponse.GetResponseStream().Returns(responseStream_);
                                }
                            }
                            else
                            {
                                webResponse.StatusCode.Returns(statusCode_.Value);
                                webResponse.GetResponseStream().Returns(responseStream_);
                            }
                            return webResponse;
                        });
                    RequestUrls.Add(url.AbsoluteUri);
                    Timings.Add(stopwatch_.Elapsed);
                    stopwatch_ = Stopwatch.StartNew();
                    return webRequest;
                }
            }
        }
    }
}
