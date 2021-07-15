using System;

namespace Applitools
{
    public class ProxySettings
    {
        public ProxySettings() { }

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
                builder.Port = Port;
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
    }
}