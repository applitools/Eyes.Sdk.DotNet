using System;

namespace Applitools
{
    public class DownloadListener<T>
    {
        public Action<T,string> OnDownloadComplete;
        public Action OnDownloadFailed;

        public DownloadListener(Action<T, string> onDownloadComplete, Action onDownloadFailed)
        {
            OnDownloadComplete = onDownloadComplete;
            OnDownloadFailed = onDownloadFailed;
        }
    }
}