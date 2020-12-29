namespace Applitools
{
    public interface ICheckTask
    {
        ICheckSettings CheckSettings { get; }
        string Source { get; }
    }
}