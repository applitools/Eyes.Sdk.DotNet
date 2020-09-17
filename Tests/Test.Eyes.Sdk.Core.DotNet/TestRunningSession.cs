using NUnit.Framework;
using System.Net;
using System;
using System.Drawing;
using Applitools.Utils;
using Newtonsoft.Json;
using NSubstitute;
using System.IO;
using System.Text;

namespace Applitools
{
    [TestFixture]
    public class TestRunningSession
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
            serverConnector.ServerUrl = new Uri(EyesBase.DefaultServerUrl);
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
                properties: sessionProperties);

            return sessionStartInfo;
        }

        [TestCase(HttpStatusCode.Created)]
        [TestCase(HttpStatusCode.OK)]
        public void TestStartSession_LongRequest_StatusCode(HttpStatusCode statusCode)
        {
            Logger logger = new Logger();
            ServerConnector serverConnector = new ServerConnector(logger);
            serverConnector.ServerUrl = new Uri(EyesBase.DefaultServerUrl);
            serverConnector.HttpRestClientFactory = new MockHttpRestClientFactory(statusCode: statusCode);
            serverConnector.ApiKey = "testKey";
            SessionStartInfo startInfo = GetStartInfo();
            RunningSession result = serverConnector.StartSession(startInfo);

            bool isNew = statusCode == HttpStatusCode.Created;
            Assert.AreEqual(isNew, result.IsNewSession);
        }

        private class MockHttpRestClientFactory : IHttpRestClientFactory
        {
            private bool? isNew_;
            private HttpStatusCode? statusCode_;

            public MockHttpRestClientFactory(bool? isNew = null, HttpStatusCode? statusCode = null)
            {
                isNew_ = isNew;
                statusCode_ = statusCode;
            }

            public HttpRestClient Create(Uri serverUrl, string agentId, JsonSerializer jsonSerializer)
            {
                var client = new HttpRestClient(serverUrl, agentId, jsonSerializer);
                client.WebRequestCreator = new MockWebRequestCreator(isNew_, statusCode_);
                return client;
            }

            private class MockWebRequestCreator : IWebRequestCreate
            {
                private Stream responseStream_;
                private int counter_ = 0;
                private HttpStatusCode? statusCode_;

                public MockWebRequestCreator(bool? isNew, HttpStatusCode? statusCode)
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
                    string responseObjJson = JsonConvert.SerializeObject(runningSession);
                    byte[] byteArray = Encoding.UTF8.GetBytes(responseObjJson);
                    responseStream_ = new MemoryStream(byteArray);
                }

                public WebRequest Create(Uri uri)
                {
                    var webRequest = Substitute.For<HttpWebRequest>();
                    webRequest.Headers.Returns(new WebHeaderCollection());
                    webRequest.GetRequestStream().Returns(new MemoryStream(new byte[2000]));

                    var webResponse = Substitute.For<HttpWebResponse>();
                    WebHeaderCollection headers = new WebHeaderCollection();
                    webResponse.Headers.Returns(headers);
                    if (statusCode_ == null)
                    {
                        if (uri.PathAndQuery.StartsWith("/api/sessions/running", StringComparison.OrdinalIgnoreCase))
                        {
                            webResponse.StatusCode.Returns(HttpStatusCode.Accepted);
                            headers.Add("Location", EyesBase.DefaultServerUrl + BASE_LOCATION + "status");
                        }
                        else if (uri.PathAndQuery.StartsWith("/" + BASE_LOCATION + "status", StringComparison.OrdinalIgnoreCase))
                        {
                            if (counter_++ == 0)
                            {
                                webResponse.StatusCode.Returns(HttpStatusCode.OK);
                            }
                            else
                            {
                                webResponse.StatusCode.Returns(HttpStatusCode.Created);
                                headers.Add("Location", EyesBase.DefaultServerUrl + BASE_LOCATION + "result");
                            }
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
                    webRequest.GetResponse().Returns(webResponse);
                    return webRequest;
                }
            }
        }
    }
}
