namespace Applitools.VisualGrid
{
    public interface IVGResource
    {
        string ContentType { get; }
        string Sha256 { get; }
        string HashFormat { get; }
        int? ErrorStatusCode { get; }
    }
}
