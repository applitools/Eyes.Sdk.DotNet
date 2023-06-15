using Applitools.Universal.Driver;
using Microsoft.Playwright;

namespace Applitools.Playwright.Universal.Driver
{
    public class TCookie : Cookie, ICookie
    {
        public TCookie(BrowserContextCookiesResult cookie)
            : this(cookie.Name, cookie.Value)
        {
            Expires = cookie.Expires;
            Domain = cookie.Domain;
            Path = cookie.Path;
            Secure = cookie.Secure;
            HttpOnly = cookie.HttpOnly;
            SameSite = cookie.SameSite;
        }

        public TCookie(Cookie cookie)
            : this(cookie.Name, cookie.Value)
        {
            Expires = cookie.Expires;
            Domain = cookie.Domain;
            Path = cookie.Path;
            Url = cookie.Url;
            Secure = cookie.Secure;
            HttpOnly = cookie.HttpOnly;
            SameSite = cookie.SameSite;
        }

        private TCookie(string name, string value)
        {
            Name = name;
            Value = value;
        }
    }
}