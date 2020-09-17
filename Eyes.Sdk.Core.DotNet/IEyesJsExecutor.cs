namespace Applitools.Utils
{
    public interface IEyesJsExecutor
    {
        object ExecuteScript(string script, params object[] args);
    }
}
