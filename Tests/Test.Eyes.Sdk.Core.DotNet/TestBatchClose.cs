using NUnit.Framework;
using System;
using System.Collections.Generic;
using NSubstitute;
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

            BatchClose batchClose = new BatchClose();
            EnabledBatchClose enabledBatchClose = batchClose.SetUrl(serverUrl1).SetBatchId(batchIds);
            Assert.AreEqual(serverUrl1, enabledBatchClose.ServerUrl);
            enabledBatchClose.SetUrl(serverUrl2);
            Assert.AreEqual(serverUrl2, enabledBatchClose.ServerUrl);

            ServerConnector serverConnector = Substitute.For<ServerConnector>(new Logger(), null);
            enabledBatchClose.serverConnector_ = serverConnector;

            enabledBatchClose.Close();
            serverConnector.Received().CloseBatch("first", serverUrl2);
            serverConnector.Received().CloseBatch("second", serverUrl2);
            serverConnector.Received().CloseBatch("third", serverUrl2);
        }
    }
}
