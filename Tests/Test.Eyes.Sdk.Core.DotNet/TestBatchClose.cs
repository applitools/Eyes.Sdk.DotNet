using NUnit.Framework;
using System;
using System.Collections.Generic;
using Applitools.Tests.Utils;

namespace Applitools.Tests
{
    [Parallelizable(ParallelScope.All)]
    public class TestBatchCloseClass : ReportingTestSuite
    {
        [Test]
        public void TestBatchClose()
        {
            List<string> batchIds = new List<string>();
            batchIds.Add("first");
            batchIds.Add("second");
            batchIds.Add("third");

            Uri serverUrl1 = new Uri("http://customUrl1");
            Uri serverUrl2 = new Uri("http://customUrl2");

            Logger logger = new Logger();
            logger.SetLogHandler(TestUtils.InitLogHandler());
            BatchClose batchClose = new BatchClose(logger);
            batchClose.ApiKey = "someApiKey";
            EnabledBatchClose enabledBatchClose = batchClose.SetUrl(serverUrl1).SetBatchId(batchIds);
            Assert.AreEqual(serverUrl1, enabledBatchClose.ServerUrl);
            enabledBatchClose.SetUrl(serverUrl2);
            Assert.AreEqual(serverUrl2, enabledBatchClose.ServerUrl);

            MockServerConnector serverConnector = new MockServerConnector(logger, null);
            enabledBatchClose.serverConnector_ = serverConnector;

            enabledBatchClose.Close();

            Assert.AreEqual(3, serverConnector.CloseBatchRequestUris.Count);
            Assert.AreEqual("http://customurl2/api/sessions/batches/first/close/bypointerid?apiKey=someApiKey", serverConnector.CloseBatchRequestUris[0].AbsoluteUri);
            Assert.AreEqual("http://customurl2/api/sessions/batches/second/close/bypointerid?apiKey=someApiKey", serverConnector.CloseBatchRequestUris[1].AbsoluteUri);
            Assert.AreEqual("http://customurl2/api/sessions/batches/third/close/bypointerid?apiKey=someApiKey", serverConnector.CloseBatchRequestUris[2].AbsoluteUri);
        }
    }
}
