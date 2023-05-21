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

        public void SetWebDriverProxy(ProxySettings proxySettings)
        {
            if (proxySettings == null)
            {
                return;
            }
            
            // this.proxy = new ProxyDto();
            // setUrl(webDriverProxy.getUrl());
            // setUsername(webDriverProxy.getUsername());
            // setPassword(webDriverProxy.getPassword());
        }
    }
}