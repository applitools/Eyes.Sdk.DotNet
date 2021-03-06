﻿using Applitools.Utils;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Applitools
{
    public abstract class EyesRunner
    {
        private readonly ConcurrentDictionary<string, IBatchCloser> batchClosers_ = new ConcurrentDictionary<string, IBatchCloser>();

        public EyesRunner()
        {
            ServerConnectorFactory = new ServerConnectorFactory();
            ServerConnector = ServerConnectorFactory.CreateNewServerConnector(Logger, new Uri(ServerUrl));
        }

        protected abstract IEnumerable<IEyesBase> GetAllEyes();

        public Logger Logger { get; } = new Logger();

        public TestResultsSummary GetAllTestResults()
        {
            return GetAllTestResults(true);
        }

        public TestResultsSummary GetAllTestResults(bool shouldThrowException)
        {
            Logger.Log(Stage.Close, StageType.TestResults);

            TestResultsSummary allTestResults;
            try
            {
                GetAllTestResultsAlreadyCalled = true;
                allTestResults = GetAllTestResultsImpl(shouldThrowException);
            }
            finally
            {
                DeleteAllBatches_();
            }
            string[] testIds = GetAllEyes().SelectMany(e => e.GetAllTests().Keys).ToArray();
            Logger.Log(TraceLevel.Notice, testIds, Stage.Close, StageType.TestResults, new { allTestResults.Count });
            return allTestResults;
        }

        private void DeleteAllBatches_()
        {
            if (DontCloseBatches) return;
            Logger.Log(TraceLevel.Notice, Stage.Close, StageType.CloseBatch, new { batches = batchClosers_.Keys });
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

        public IServerConnector ServerConnector { get; set; }
        public IServerConnectorFactory ServerConnectorFactory { get; set; }

        public string ApiKey
        {
            get => ServerConnector.ApiKey;
            set => ServerConnector.ApiKey = value;
        }

        public string ServerUrl
        {
            get => ServerConnector?.ServerUrl.AbsoluteUri ?? CommonUtils.ServerUrl;
            set => ServerConnector.ServerUrl = new Uri(value);
        }

        public ProxySettings Proxy
        {
            get => ServerConnector.Proxy;
            set => ServerConnector.Proxy = value;
        }

        public bool IsDisabled { get; set; }
        public bool DontCloseBatches { get; set; } = CommonUtils.DontCloseBatches;
        public string AgentId
        {
            get { return ServerConnector.AgentId; }
            set { if (value != null) ServerConnector.AgentId = value; }
        }

        public bool GetAllTestResultsAlreadyCalled { get; private set; } = false;
    }
}