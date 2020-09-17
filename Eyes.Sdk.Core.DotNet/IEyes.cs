namespace Applitools
{
    public interface IEyes : IEyesBase
    {
        void Check(ICheckSettings checkSettings);

        void Check(params ICheckSettings[] checkSettings);
    }
}
