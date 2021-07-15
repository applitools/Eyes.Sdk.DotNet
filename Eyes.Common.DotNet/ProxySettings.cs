using System;
using System.Net;

namespace Applitools
{
    public class ProxySettings
    {
        public ProxySettings() { }

        public ProxySettings(WebProxy webProxy)
        {
            Uri addr = webProxy.Address;
            Address = addr.Scheme + "://" + addr.Host + addr.PathAndQuery;
            Port = addr.Port;
            if (addr.UserInfo.Length > 0)
            {
                string[] userAndPass = addr.UserInfo.Split(':');
                if (userAndPass?.Length > 0) Username = userAndPass[0];
                if (userAndPass?.Length > 1) Password = userAndPass[1];
            }
        }

        public ProxySettings(string address)
        {
            Address = address;
        }

        public ProxySettings(string address, int port, string username, string password)
        {
            Address = address;
            Port = port;
            Username = username;
            Password = password;
        }

        public string Address { get; set; }
        public int Port { get; set; } = 80;
        public string Username { get; set; }
        public string Password { get; set; }

        public Uri ProxyUri
        {
            get
            {
                UriBuilder builder = new UriBuilder(Address);
                if (builder.Port == -1)
                {
                    builder.Port = Port;
                }
                if (Username != null)
                {
                    builder.UserName = Username;
                }
                if (Password != null)
                {
                    builder.Password = Password;
                }
                return builder.Uri;
            }
        }

        public static implicit operator WebProxy(ProxySettings proxySettings)
        {
            return new WebProxy(proxySettings.ProxyUri);
        }

        public static implicit operator ProxySettings(WebProxy webProxy)
        {
            return new ProxySettings(webProxy);
        }
    }
}