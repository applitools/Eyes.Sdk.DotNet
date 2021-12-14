namespace Eyes.Universal.Sdk.DotNet
{
    public interface IUniversalClient
    {
        void Open(string appName, string testName);
        void Check(string name);
        void Close();
    }
}
