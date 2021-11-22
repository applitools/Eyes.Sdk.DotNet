namespace Applitools.VisualGrid
{
    internal interface IVisualGridEyesInternal
    {
        bool IsCloseIssued { get; }

        void CloseAsync();
        bool IsCompleted();
    }
}