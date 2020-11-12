using Applitools.Utils;
using System;
using System.Collections.Generic;

namespace Applitools
{
    public class BatchClose
    {
        protected readonly Logger logger_;

        public BatchClose() : this(new Logger()) { }

        public BatchClose(Logger logger)
        {
            logger_ = logger;
            ServerUrl = new Uri(CommonUtils.ServerUrl);
        }

        public Uri ServerUrl { get; protected set; }

        public BatchClose SetUrl(string url)
        {
            return SetUrl(new Uri(url));
        }

        public BatchClose SetUrl(Uri url)
        {
            ServerUrl = url;
            return this;
        }

        public EnabledBatchClose SetBatchId(IEnumerable<string> batchIds)
        {
            ArgumentGuard.NotContainsNull(batchIds, nameof(batchIds));
            return new EnabledBatchClose(logger_, serverUrl_, batchIds);
        }
    }
}
