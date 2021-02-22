using System;
using System.IO;
using System.Net;
using System.Threading;

namespace Applitools.Utils
{
    internal class UploadCallback : TaskListener<HttpWebResponse>
    {
        private static readonly TimeSpan UPLOAD_TIMEOUT = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan TIME_THRESHOLD = TimeSpan.FromSeconds(20);

        private readonly TaskListener<string> listener_;
        private readonly ServerConnector serverConnector_;
        private readonly string targetUrl_;
        private readonly byte[] bytes_;
        private readonly string contentType_;
        private readonly string mediaType_;
        private readonly string[] testIds_;

        private TimeSpan sleepDuration = TimeSpan.FromSeconds(5);
        private TimeSpan timePassed = TimeSpan.Zero;

        public UploadCallback(TaskListener<string> listener, ServerConnector serverConnector,
                          string targetUrl, byte[] bytes, string contentType, string mediaType, string[] testIds)
        {
            ArgumentGuard.NotNull(bytes, nameof(bytes));
            OnComplete = OnComplete_;
            OnFail = OnFail_;
            listener_ = listener;
            serverConnector_ = serverConnector;
            targetUrl_ = targetUrl;
            bytes_ = bytes;
            contentType_ = contentType;
            mediaType_ = mediaType;
            testIds_ = testIds;
            HttpRestClient client = serverConnector.CreateHttpRestClient(new Uri(targetUrl));
            WebRequestCreator = client.WebRequestCreator;
        }

        internal IWebRequestCreate WebRequestCreator { get; set; } = DefaultWebRequestCreator.Instance;


        private void OnComplete_(HttpWebResponse response)
        {
            HttpStatusCode statusCode = response.StatusCode;
            string statusPhrase = response.StatusDescription;
            response.Close();

            if (statusCode == HttpStatusCode.OK || statusCode == HttpStatusCode.Created)
            {
                listener_.OnComplete(targetUrl_);
                return;
            }

            string errorMessage = $"Status: {statusCode} {statusPhrase}.";

            if ((int)statusCode < 500)
            {
                OnFail_(new IOException($"Failed uploading image. {errorMessage}"));
                return;
            }

            if (timePassed >= UPLOAD_TIMEOUT)
            {
                OnFail_(new IOException("Failed uploading image"));
                return;
            }

            if (timePassed >= TIME_THRESHOLD)
            {
                sleepDuration = TimeSpan.FromSeconds(10);
            }

            Thread.Sleep(sleepDuration);

            timePassed += sleepDuration;
            UploadDataAsync();
        }

        private void OnFail_(Exception ex)
        {
            CommonUtils.LogExceptionStackTrace(serverConnector_.Logger, Stage.General, StageType.UploadResource, ex);
            listener_.OnFail(ex);
        }

        public void UploadDataAsync()
        {
            HttpWebRequest request = (HttpWebRequest)WebRequestCreator.Create(new Uri(targetUrl_));
            if (serverConnector_.Proxy != null) request.Proxy = serverConnector_.Proxy;
            request.ContentType = contentType_;
            request.MediaType = mediaType_;
            request.Method = "PUT";
            request.Headers.Add("X-Auth-Token", serverConnector_.GetRenderingInfo().AccessToken);
            request.Headers.Add("x-ms-blob-type", "BlockBlob");

            request.ContentLength = bytes_.Length;
            Stream dataStream = request.GetRequestStream();
            dataStream.Write(bytes_, 0, bytes_.Length);
            dataStream.Close();

            request.BeginGetResponse(ar =>
            {
                if (!ar.IsCompleted)
                {
                    serverConnector_.Logger.Log(TraceLevel.Notice, testIds_, Stage.General, StageType.UploadResource,
                        new { message = "upload not complete" });
                    return;
                }
                HttpWebRequest resultRequest = (HttpWebRequest)ar.AsyncState;
                HttpWebResponse response = (HttpWebResponse)resultRequest.EndGetResponse(ar);
                HttpStatusCode statusCode = response.StatusCode;
                serverConnector_.Logger.Log(TraceLevel.Notice, testIds_, Stage.General, StageType.UploadComplete,
                    new { statusCode });
                try
                {
                    OnComplete_(response);
                }
                finally
                {
                    response.Close();
                }
            }, request);
        }
    }
}
