using Applitools.Ufg.Model;
using System;

namespace Applitools.Selenium
{
    internal class Cookie : ICookie, IEquatable<Cookie>
    {
        public Cookie()
        {
        }

        public Cookie(string name, string value, string path, string domain)
        {
            Name = name;
            Value = value;
            Path = path;
            Domain = domain;
        }

        public string Name { get; set; }
        public string Value { get; set; }
        public string Path { get; set; }
        public string Domain { get; set; }
        public bool Secure { get; set; }
        public bool HttpOnly { get; set; }

        public bool Expired
        {
            get
            {
                if (Expiry.HasValue)
                    return Expiry.Value.ToUniversalTime() <= DateTime.UtcNow;
                else
                    return false;
            }
        }

        public DateTime? Expiry { get; set; }

        public bool Equals(Cookie other)
        {
            return Name == other.Name && Value == other.Value && Path == other.Path && Domain == other.Domain;
        }

        public override string ToString()
        {
            return $"{Name}={Value} ({Domain} ; {Path} ; {Expiry})";
        }
    }
}