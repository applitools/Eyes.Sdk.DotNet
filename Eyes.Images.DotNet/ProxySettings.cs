using System;
using System.Net;
using Newtonsoft.Json;

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

        public ProxySettings(string address, int port, string username = null, string password = null)
        {
            Address = address;
            Port = port;
            Username = username;
            Password = password;
        }

        [JsonIgnore]
        public string Address { get; set; }
        [JsonIgnore]
        public int Port { get; set; } = 80;
        public string Username { get; set; }
        public string Password { get; set; }
        public bool? IsHttpOnly { get; set; }

        public Uri Url
        {
            get
            {
                UriBuilder builder = new UriBuilder(Address);
                if (builder.Port == 80 && Port != 80)
                {
                    builder.Port = Port;
                }
                if (Username != null)
                {
                    builder.UserName = Uri.EscapeDataString(Username);
                }
                if (Password != null)
                {
                    builder.Password = Uri.EscapeDataString(Password);
                }
                return builder.Uri;
            }
        }

        public static implicit operator WebProxy(ProxySettings proxySettings)
        {
            if (proxySettings == null) return null;
            Uri proxyUri = proxySettings.Url;
            WebProxy proxy = new WebProxy(proxyUri);
            if (!string.IsNullOrEmpty(proxyUri.UserInfo))
            {
                string[] userInfoParts = proxyUri.UserInfo.Split(':');
                var creds = new NetworkCredential();
                if (userInfoParts.Length > 0)
                {
                    creds.UserName = userInfoParts[0];
                }
                if (userInfoParts.Length > 1)
                {
                    creds.Password = userInfoParts[1];
                }
                proxy.Credentials = creds;
            }
            return proxy;
        }

        public static implicit operator ProxySettings(WebProxy webProxy)
        {
            if (webProxy == null) return null;
            return new ProxySettings(webProxy);
        }

        public override string ToString()
        {
            return Url.ToString();
        }
    }
}