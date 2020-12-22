namespace Applitools.VisualGrid
{
    public interface IRenderStatusResults
    {
        RenderStatus Status { get; set; }
        string Error { get; set; }
    }
}
