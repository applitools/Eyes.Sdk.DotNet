namespace Applitools
{
    public interface IEyesBase : Common.IEyesBase
    {
        BatchInfo Batch { get; set; }
     
        TestResults Abort();

        TestResults AbortIfNotClosed();
    }
}
