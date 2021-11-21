using System;

namespace Applitools.OpenQA.Selenium
{
    public interface INavigation
    {
        void Back();
        void Forward();
        void GoToUrl(Uri url);
        void GoToUrl(string url);
        void Refresh();
    }
}
