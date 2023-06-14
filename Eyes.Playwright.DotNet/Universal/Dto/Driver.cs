using System;
using Applitools.Commands;
using Applitools.Universal;
using Microsoft.Playwright;
using Newtonsoft.Json;

namespace Applitools.Playwright.Universal.Dto
{
    public class Driver : Reference, ITarget
    {
        [JsonIgnore]
        public IPage Page { get; set; }

        [JsonIgnore]
        public Reference Root { get; }
        public ProxySettings Proxy { get; set; }

        public Driver()
        {
            Root = new Reference
            {
                ApplitoolsRefId = Guid.NewGuid().ToString()
            };
        }

        public Driver(IPage page)
        {
            Page = page;

            Root = new Driver();
            Root.ApplitoolsRefId = Guid.NewGuid().ToString();
        }
    }
}