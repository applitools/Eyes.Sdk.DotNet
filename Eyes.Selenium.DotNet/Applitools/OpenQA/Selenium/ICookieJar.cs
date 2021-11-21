using Applitools.Selenium;
using System.Collections.ObjectModel;

namespace Applitools.OpenQA.Selenium
{
    public interface ICookieJar
    {
        ReadOnlyCollection<Cookie> AllCookies { get; }
    }
}