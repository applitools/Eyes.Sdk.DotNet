namespace Applitools.OpenQA.Selenium
{
    public interface IJavaScriptExecutor
    {
        object ExecuteScript(string script, params object[] args);
        object ExecuteAsyncScript(string script, object[] args);
    }
}