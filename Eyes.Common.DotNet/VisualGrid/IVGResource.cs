using System;

namespace Applitools.VisualGrid
{
    public interface IVGResource
    {
        byte[] Content { get; }
        string ContentType { get; }
        string Sha256 { get; }
        string HashFormat { get; }
        int? ErrorStatusCode { get; }
        Uri Url { get; }
    }
}
