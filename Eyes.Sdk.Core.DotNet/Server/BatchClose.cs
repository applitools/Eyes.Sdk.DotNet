using Applitools.Utils;
using System;
using System.Collections.Generic;
using System.Net;

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

        public Uri ServerUrl { get; set; }
        public WebProxy Proxy { get;set; }
        public string ApiKey { get; set; }

        public virtual BatchClose SetUrl(string url)
        {
            return SetUrl(new Uri(url));
        }

        public virtual BatchClose SetUrl(Uri url)
        {
            ServerUrl = url;
            return this;
        }

        public BatchClose SetApiKey(string apiKey)
        {
            ArgumentGuard.NotNull(apiKey, nameof(apiKey));
            ApiKey = apiKey;
            return this;
        }

        public BatchClose SetProxy(ProxySettings proxy)
        {
            WebProxy webProxy = new WebProxy(proxy.ProxyUri);
            return SetProxy(webProxy);
        }

        public BatchClose SetProxy(WebProxy proxy)
        {
            ArgumentGuard.NotNull(proxy, nameof(proxy));
            Proxy = proxy;
            return this;
        }

        public EnabledBatchClose SetBatchId(params string[] batchIds)
        {
            return SetBatchId((IEnumerable<string>)batchIds);
        }

        public EnabledBatchClose SetBatchId(IEnumerable<string> batchIds)
        {
            ArgumentGuard.NotContainsNull(batchIds, nameof(batchIds));
            EnabledBatchClose enabledBatchClose = new EnabledBatchClose(logger_, ServerUrl, batchIds);
           
            if (ApiKey != null)
            {
                enabledBatchClose.SetApiKey(ApiKey);
            }

            if (Proxy != null)
            {
                enabledBatchClose.SetProxy(Proxy);
            }

            return enabledBatchClose;
        }
    }
}
