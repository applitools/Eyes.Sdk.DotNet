using OpenQA.Selenium;
using System.Drawing;

namespace Applitools.Selenium
{
    internal interface IUserActionsEyes
    {
        void AddKeyboardTrigger(IWebElement element, string text);
        void AddMouseTrigger(MouseAction action, IWebElement element, Point cursor);
    }
}