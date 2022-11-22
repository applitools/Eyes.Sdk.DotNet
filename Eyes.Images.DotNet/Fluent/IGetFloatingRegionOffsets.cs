namespace Applitools
{
    public interface IGetFloatingRegionOffsets
    {
        int MaxLeftOffset { get; }
        int MaxUpOffset { get; }
        int MaxRightOffset { get; }
        int MaxDownOffset { get; }
    }
}