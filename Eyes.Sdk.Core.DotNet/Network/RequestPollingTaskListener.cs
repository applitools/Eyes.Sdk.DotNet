using System;
using System.Net;
using System.Threading;

namespace Applitools.Utils
{
    internal class RequestPollingTaskListener : TaskListener<HttpWebResponse>
    {
        private HttpRestClient restClient;
        private Logger logger;
        private string pollingUrl;
        private TaskListener<HttpWebResponse> listener;
        private int sleepDuration = 500;
        private int requestCount;

        public RequestPollingTaskListener(HttpRestClient restClient, string pollingUrl, 
            TaskListener<HttpWebResponse> listener, Logger logger = null)
        {
            this.restClient = restClient;
            this.logger = logger;
            this.pollingUrl = pollingUrl;
            this.listener = listener;
            OnComplete = OnComplete_;
            OnFail = OnFail_;
        }

        private void OnComplete_(HttpWebResponse response)
        {
            string location = response.Headers[HttpResponseHeader.Location];
            try
            {
                HttpStatusCode status = response.StatusCode;
                if (status == HttpStatusCode.Created)
                {
                    logger?.Verbose("exit (CREATED)");
                    restClient.SendAsyncRequest(listener, location, "DELETE");
                    return;
                }

                if (status != HttpStatusCode.OK)
                {
                    listener.OnFail(new EyesException($"Got bad status code when polling from the server. Status code: {status}"));
                    return;
                }
            }
            finally
            {
                response.Close();
            }

            if (location != null)
            {
                pollingUrl = location;
            }

            int timeToWait = sleepDuration;
            string secondsToWait = response.Headers[HttpResponseHeader.RetryAfter];
            if (secondsToWait != null)
            {
                timeToWait = int.Parse(secondsToWait) * 1000;
            }
            else if (requestCount++ >= 5)
            {
                sleepDuration *= 2;
                requestCount = 0;
                sleepDuration = Math.Min(5000, sleepDuration);
            }

            Thread.Sleep(timeToWait);
            logger?.Verbose("polling...");
            restClient.SendAsyncRequest(this, pollingUrl, "GET");
        }

        private void OnFail_(Exception ex)
        {
            listener.OnFail(ex);
        }
    }
}
