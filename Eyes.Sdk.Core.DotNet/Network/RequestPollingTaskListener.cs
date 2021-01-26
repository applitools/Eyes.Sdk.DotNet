using System;
using System.Net;
using System.Threading;

namespace Applitools.Utils
{
    internal class RequestPollingTaskListener : TaskListener<HttpWebResponse>
    {
        private HttpRestClient restClient_;
        private Logger logger_;
        private string pollingUrl_;
        private TaskListener<HttpWebResponse> listener_;
        private int sleepDuration_ = 500;
        private int requestCount_;

        public RequestPollingTaskListener(HttpRestClient restClient, string pollingUrl, 
            TaskListener<HttpWebResponse> listener, Logger logger = null)
        {
            restClient_ = restClient;
            logger_ = logger;
            pollingUrl_ = pollingUrl;
            listener_ = listener;
            OnComplete = OnComplete_;
            OnFail = OnFail_;
        }

        private void OnComplete_(HttpWebResponse response)
        {
            string location = response.Headers[HttpResponseHeader.Location];
            string secondsToWait = response.Headers[HttpResponseHeader.RetryAfter];
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
                response.Close();
            }

            if (location != null)
            {
                pollingUrl_ = location;
            }

            int timeToWait = sleepDuration_;
            if (secondsToWait != null)
            {
                timeToWait = int.Parse(secondsToWait) * 1000;
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
