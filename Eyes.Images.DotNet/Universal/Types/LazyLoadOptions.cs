namespace Applitools
{
    public class LazyLoadOptions
    {
        public int? ScrollLength { get; }
        public int? WaitingTime { get; }
        public int? MaxAmountToScroll { get; }

        public LazyLoadOptions(int? scrollLength = null, int? waitingTime = null, int? maxAmountToScroll = null)
        {
            ScrollLength = scrollLength;
            WaitingTime = waitingTime;
            MaxAmountToScroll = maxAmountToScroll;
        }
    }
}