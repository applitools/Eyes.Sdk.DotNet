using System;
using Applitools.Universal;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Dto
{
    public class Driver : Reference
    {
        public IPage Page { get; set; }
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