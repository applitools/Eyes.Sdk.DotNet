using System;

namespace Applitools
{
    internal class LeanFTJavaScriptExecutor : Utils.IEyesJsExecutor
    {
        private readonly Func<string, object> executeScript_;

        public LeanFTJavaScriptExecutor(Func<string, object> executeScript)
        {
            executeScript_ = executeScript;
        }

        public object ExecuteScript(string script, params object[] args)
        {
            return executeScript_(script);
        }
    }
}