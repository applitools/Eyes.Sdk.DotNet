﻿using Applitools.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Applitools
{
    public abstract class EyesRunner
    {
        private readonly ConcurrentDictionary<string, IBatchCloser> batchClosers_ = new ConcurrentDictionary<string, IBatchCloser>();

        public Logger Logger { get; } = new Logger();

        public TestResultsSummary GetAllTestResults()
        {
            return GetAllTestResults(true);
        }

        public TestResultsSummary GetAllTestResults(bool shouldThrowException)
        {
            TestResultsSummary allTestResults;
            try
            {
                allTestResults = GetAllTestResultsImpl(shouldThrowException);
            }
            finally
            {
                DeleteAllBatches_();
            }
            return allTestResults;
        }

        private void DeleteAllBatches_()
        {
            if (DontCloseBatches) return;
            foreach (KeyValuePair<string, IBatchCloser> kvp in batchClosers_)
            {
                IBatchCloser connector = kvp.Value;
                connector.CloseBatch(kvp.Key);
            }
        }

        public void AddBatch(string batchId, IBatchCloser batchCloser)
        {
            batchClosers_.TryAdd(batchId, batchCloser);
        }

        protected abstract TestResultsSummary GetAllTestResultsImpl(bool shouldThrowException);

        public void SetLogHandler(ILogHandler logHandler)
        {
            Logger.SetLogHandler(logHandler);
            if (!logHandler.IsOpen)
            {
                logHandler.Open();
            }
        }

        public string ApiKey { get; set; } = CommonUtils.GetEnvVar("APPLITOOLS_API_KEY");
        public string ServerUrl { get; set; } = CommonUtils.ServerUrl;
        public bool IsDisabled { get; set; }
        public bool DontCloseBatches { get; set; } = "true".Equals(CommonUtils.GetEnvVar("APPLITOOLS_DONT_CLOSE_BATCHES"), StringComparison.OrdinalIgnoreCase);
    }
}