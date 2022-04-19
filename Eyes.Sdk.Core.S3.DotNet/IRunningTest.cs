namespace Applitools
{
    public interface IRunningTest : IBatchCloser
    {
        bool IsCompleted { get; }
    }
}