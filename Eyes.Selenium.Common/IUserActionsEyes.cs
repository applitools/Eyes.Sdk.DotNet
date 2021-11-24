using System.Drawing;

namespace Applitools.Selenium
{
    internal interface IUserActionsEyes<IWebElement>
    {
        void AddKeyboardTrigger(IWebElement element, string text);
        void AddMouseTrigger(MouseAction action, IWebElement element, Point cursor);
    }
}