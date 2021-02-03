using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Applitools
{
    public class EnabledBatchClose : BatchClose
    {
        internal ServerConnector serverConnector_;
        private IEnumerable<string> batchIds_;

        public EnabledBatchClose(Logger logger, Uri serverUrl, IEnumerable<string> batchIds) : base(logger)
        {
            serverConnector_ = new ServerConnector(logger);
            ServerUrl = serverUrl;
            batchIds_ = batchIds;
        }

        public new EnabledBatchClose SetUrl(string url)
        {
            return SetUrl(new Uri(url));
        }

        public new EnabledBatchClose SetUrl(Uri url)
        {
            ServerUrl = url;
            return this;
        }

        public new EnabledBatchClose SetBatchId(IEnumerable<string> batchIds)
        {
            ArgumentGuard.NotContainsNull(batchIds, nameof(batchIds));
            batchIds_ = batchIds;
            return this;
        }

        public void Close()
        {
            logger_.Log(TraceLevel.Notice, null, Stage.Close, StageType.CloseBatch, new { batches = batchIds_ });
            serverConnector_.ApiKey = ApiKey;
            serverConnector_.Proxy = Proxy;
            foreach (string batchId in batchIds_)
            {
                serverConnector_.CloseBatch(batchId, ServerUrl);
            }
        }
    }
}