namespace Applitools.OpenQA.Selenium
{
    public interface IOptions
    {
        IWindow Window { get; }
        ICookieJar Cookies { get; }
    }
}