using Applitools.Commands;

namespace Applitools
{
    public interface ISpecDriverMessageAdaptor
    {
        TResponse HandleMessage<TResponse>(string message) where TResponse : CommandBase;
    }
}