using System;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Applitools.Utils
{
    internal class UploadCallback : TaskListener<HttpResponseMessage>
    {
        private static readonly TimeSpan UPLOAD_TIMEOUT = TimeSpan.FromSeconds(60);
        private static readonly TimeSpan TIME_THRESHOLD = TimeSpan.FromSeconds(20);

        private readonly TaskListener<string> listener_;
        private readonly ServerConnector serverConnector_;
        private readonly Logger logger_;
        private readonly string targetUrl_;
        private readonly byte[] bytes_;
        private readonly string contentType_;
        private readonly string mediaType_;
        private readonly string[] testIds_;

        private TimeSpan sleepDuration_ = TimeSpan.FromSeconds(5);
        private TimeSpan timePassed_ = TimeSpan.Zero;

        public UploadCallback(TaskListener<string> listener, ServerConnector serverConnector,
                          string targetUrl, byte[] bytes, string contentType, string mediaType, string[] testIds)
        {
            ArgumentGuard.NotNull(bytes, nameof(bytes));
            OnComplete = OnComplete_;
            OnFail = OnFail_;
            listener_ = listener;
            serverConnector_ = serverConnector;
            logger_ = serverConnector.Logger;
            targetUrl_ = targetUrl;
            bytes_ = bytes;
            contentType_ = contentType;
            mediaType_ = mediaType;
            testIds_ = testIds;
        }

        private void OnComplete_(HttpResponseMessage response)
        {
            HttpStatusCode statusCode = response.StatusCode;
            string statusPhrase = response.ReasonPhrase;
            response.Dispose();

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

            if (timePassed_ >= UPLOAD_TIMEOUT)
            {
                OnFail_(new IOException("Failed uploading image"));
                return;
            }

            if (timePassed_ >= TIME_THRESHOLD)
            {
                sleepDuration_ = TimeSpan.FromSeconds(10);
            }

            Thread.Sleep(sleepDuration_);

            timePassed_ += sleepDuration_;
            UploadDataAsync();
        }

        private void OnFail_(Exception ex)
        {
            CommonUtils.LogExceptionStackTrace(serverConnector_.Logger, Stage.General, StageType.UploadResource, ex);
            listener_.OnFail(ex);
        }

        public void UploadDataAsync()
        {
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, new Uri(targetUrl_));
            request.Content = new ByteArrayContent(bytes_);
            request.Content.Headers.ContentType = new MediaTypeHeaderValue(contentType_);
            request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue(mediaType_));
            request.Headers.Add("X-Auth-Token", serverConnector_.GetRenderingInfo().AccessToken);
            request.Headers.Add("x-ms-blob-type", "BlockBlob");

            IAsyncResult asyncResult = serverConnector_.httpClient_.GetHttpClient().SendAsync(request).AsApm(
                ar =>
                {
                    if (!ar.IsCompleted)
                    {
                        serverConnector_.Logger.Log(TraceLevel.Notice, testIds_, Stage.General, StageType.UploadResource,
                            new { message = "upload not complete" });
                        return;
                    }
                    HandleResult_(ar);
                }, request);

            if (asyncResult != null)
            {
                serverConnector_.Logger.Log(TraceLevel.Notice, Stage.General,
                    new { message = "request.BeginGetResponse returned immediately", asyncResult });

                if (asyncResult.CompletedSynchronously)
                {
                    serverConnector_.Logger.Log(TraceLevel.Notice, Stage.General,
                       new { message = "request.BeginGetResponse completed synchronously" });
                    HandleResult_(asyncResult);
                }
            }
        }

        private void HandleResult_(IAsyncResult result)
        {
            HttpResponseMessage response = ((Task<HttpResponseMessage>)result).Result;
            HttpStatusCode statusCode = response.StatusCode;
            serverConnector_.Logger.Log(TraceLevel.Notice, testIds_, Stage.General, StageType.UploadComplete,
                new { statusCode });
            try
            {
                OnComplete_(response);
            }
            finally
            {
                response.Dispose();
            }
        }
    }
}
