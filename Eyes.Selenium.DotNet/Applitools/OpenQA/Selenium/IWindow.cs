using System.Drawing;

namespace Applitools.OpenQA.Selenium
{
    public interface IWindow
    {
        Size Size { get; set; }

        Point Position { get; set; }
    }
}