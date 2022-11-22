using System;
using System.Net;
using Applitools.Fluent;

namespace Applitools
{
    public class LocateSettings : SettingsBase
    {
        public Uri ServerUrl { get; set; }
        public string ApiKey { get; set; }
        public WebProxy Proxy { get; set; }
        public TRegion Region { get; set; }
        public string AppName { get; set; }
        public object[] LocatorNames { get; set; }
        public bool? FirstOnly { get; set; }
    }
}