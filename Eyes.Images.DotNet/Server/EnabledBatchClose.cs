using System;
using System.Collections.Generic;
using System.Linq;
using Applitools.Commands.Requests;
using Applitools.Commands.Responses;
using Applitools.Utils;

namespace Applitools
{
    public class EnabledBatchClose : BatchClose
    {
        private readonly ClassicRunner _runner;
        private IEnumerable<string> _batchIds;

        public EnabledBatchClose(Logger logger, Uri serverUrl, IEnumerable<string> batchIds) : base(logger)
        {
            ServerUrl = serverUrl;
            _batchIds = batchIds;
            _runner = new ClassicRunner();
            _runner.CreateCoreSdk("netImagesBatchClose");
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
            _batchIds = batchIds;
            return this;
        }

        public void Close()
        {
            var batchIds = _batchIds.Any() ? _batchIds : new List<string> {CommonUtils.GetEnvVar("APPLITOOLS_BATCH_ID")};

            logger_.Log(TraceLevel.Notice, Stage.Close, StageType.CloseBatch, new {batches = _batchIds});
            var request = new CoreCloseBatchesRequest
            {
                Key = Guid.NewGuid().ToString(),
                Payload = new CloseBatchRequestPayload
                {
                    
                    Settings = batchIds.Select(b => new CloseBatchSettings
                    {
                        BatchId = b,
                        ApiKey = ApiKey,
                        Proxy = Proxy,
                        ServerUrl = ServerUrl.ToString()
                    }).ToList()
                }
            };

            _runner.SendRequest(request);
        }
    }
}