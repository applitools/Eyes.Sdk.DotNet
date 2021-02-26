using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;

namespace Applitools.Utils
{
    internal class RequestPollingTaskListener : TaskListener<HttpResponseMessage>
    {
        private HttpRestClient restClient_;
        private Logger logger_;
        private Uri pollingUrl_;
        private TaskListener<HttpResponseMessage> listener_;
        private int sleepDuration_ = 500;
        private int requestCount_;

        public RequestPollingTaskListener(HttpRestClient restClient, Uri pollingUrl,
            TaskListener<HttpResponseMessage> listener, Logger logger = null)
        {
            restClient_ = restClient;
            logger_ = logger;
            pollingUrl_ = pollingUrl;
            listener_ = listener;
            OnComplete = OnComplete_;
            OnFail = OnFail_;
        }

        private void OnComplete_(HttpResponseMessage response)
        {
            Uri location = response.Headers.Location;
            RetryConditionHeaderValue secondsToWait = response.Headers.RetryAfter;
            try
            {
                HttpStatusCode status = response.StatusCode;
                if (status == HttpStatusCode.Created)
                {
                    logger_?.Verbose("exit (CREATED)");
                    restClient_.SendAsyncRequest(listener_, location, "DELETE");
                    return;
                }

                if (status != HttpStatusCode.OK)
                {
                    listener_.OnFail(new EyesException($"Got bad status code when polling from the server. Status code: {status}"));
                    return;
                }
            }
            finally
            {
                response.Dispose();
            }

            if (location != null)
            {
                pollingUrl_ = location;
            }

            int timeToWait = sleepDuration_;
            if (secondsToWait != null)
            {
                timeToWait = (int)secondsToWait.Delta.Value.TotalSeconds;
            }
            else if (requestCount_++ >= 5)
            {
                sleepDuration_ *= 2;
                requestCount_ = 0;
                sleepDuration_ = Math.Min(5000, sleepDuration_);
            }

            Thread.Sleep(timeToWait);
            logger_?.Verbose("polling...");
            restClient_.SendAsyncRequest(this, pollingUrl_, "GET");
        }

        private void OnFail_(Exception ex)
        {
            listener_.OnFail(ex);
        }
    }
}
