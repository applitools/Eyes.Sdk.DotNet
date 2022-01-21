namespace Applitools.Selenium
{
    public interface IUniversalClient
    {
        void Open(string appName, string testName);
        void Check(string name);
        void Close();
    }
}
