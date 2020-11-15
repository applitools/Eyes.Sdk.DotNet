using Applitools.Tests.Utils;
using Applitools.Utils;
using Applitools.VisualGrid;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    public class TestBatchClose : ReportingTestSuite
    {

        private VisualGridRunner InitRunnerWithBatches_(Dictionary<string, IBatchCloser> batchClosers)
        {
            VisualGridRunner runner = new VisualGridRunner(10);
            foreach (var pair in batchClosers)
            {
                runner.AddBatch(pair.Key, pair.Value);
            }

            return runner;
        }

        [Test]
        public void TestBatchCloseFlag()
        {
            CommonUtils.DontCloseBatches = false;
            IBatchCloser first = Substitute.For<IBatchCloser>();
            IBatchCloser second = Substitute.For<IBatchCloser>();
            IBatchCloser third = Substitute.For<IBatchCloser>();
            Dictionary<string, IBatchCloser> batchCloserMap = new Dictionary<string, IBatchCloser>();
            batchCloserMap.Add("first", first);
            batchCloserMap.Add("second", second);
            batchCloserMap.Add("third", third);

            // default
            VisualGridRunner runner = InitRunnerWithBatches_(batchCloserMap);
            runner.GetAllTestResults();
            first.Received().CloseBatch("first");
            second.Received().CloseBatch("second");
            third.Received().CloseBatch("third");

            // set true
            first.ClearReceivedCalls();
            second.ClearReceivedCalls();
            third.ClearReceivedCalls();

            runner = InitRunnerWithBatches_(batchCloserMap);
            runner.DontCloseBatches = true;
            runner.GetAllTestResults();
            first.DidNotReceive().CloseBatch("first");
            second.DidNotReceive().CloseBatch("second");
            third.DidNotReceive().CloseBatch("third");

            // set false
            first.ClearReceivedCalls();
            second.ClearReceivedCalls();
            third.ClearReceivedCalls();
            runner = InitRunnerWithBatches_(batchCloserMap);
            runner.DontCloseBatches = false;
            runner.GetAllTestResults();
            first.Received().CloseBatch("first");
            second.Received().CloseBatch("second");
            third.Received().CloseBatch("third");
        }
    }
}
