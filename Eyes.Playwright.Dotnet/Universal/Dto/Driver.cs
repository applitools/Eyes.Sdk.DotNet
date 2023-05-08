using System;
using Applitools.Universal;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Dto
{
    public class Driver : Reference, ITargetDto
    {

        private ProxyDto proxy = null;


        public IPage Page { get; set; }
        public Reference root;

        public Driver()
        {
            super();
            root = new Reference();
            root.setApplitoolsRefId(UUID.randomUUID().toString());
        }

        public Driver(Page page)
        {
            super();
            this.page = page;

            root = new Driver();
            root.setApplitoolsRefId(UUID.randomUUID().toString());
        }

        public IPage getPage()
        {
            return page;
        }

        public void setPage(IPage page)
        {
            this.page = page;
        }

        public Reference getRoot()
        {
            return root;
        }

        public void setWebDriverProxy(WebDriverProxySettings webDriverProxy)
        {
            if (webDriverProxy == null)
            {
                return;
            }

            this.proxy = new ProxyDto();
            setUrl(webDriverProxy.getUrl());
            setUsername(webDriverProxy.getUsername());
            setPassword(webDriverProxy.getPassword());
        }

        public string getUrl()
        {
            return proxy != null ? proxy.getUrl() : null;
        }

        public void setUrl(string url)
        {
            if (proxy == null)
            {
                this.proxy = new ProxyDto();
            }

            this.proxy.setUrl(url);
        }

        public string getUsername()
        {
            return proxy != null ? proxy.getUsername() : null;
        }

        public void setUsername(string username)
        {
            if (proxy == null)
            {
                this.proxy = new ProxyDto();
            }

            this.proxy.setUsername(username);
        }

        public string getPassword()
        {
            return proxy != null ? proxy.getPassword() : null;
        }

        public void setPassword(string password)
        {
            if (proxy == null)
            {
                this.proxy = new ProxyDto();
            }

            this.proxy.setPassword(password);
        }
    }
}