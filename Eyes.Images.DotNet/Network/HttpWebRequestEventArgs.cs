using System;
using System.Net;

namespace Applitools.Utils
{
    public class HttpWebRequestEventArgs : EventArgs
    {
        public HttpWebRequestEventArgs(HttpWebRequest httpWebRequest)
        {
            this.HttpWebRequest = httpWebRequest;
        }

        public HttpWebRequest HttpWebRequest { get; private set; }
    }
}
