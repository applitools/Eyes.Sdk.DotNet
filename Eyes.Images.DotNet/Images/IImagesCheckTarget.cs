using System;
using System.Drawing;

namespace Applitools.Images
{
    public interface IImagesCheckTarget
    {
        Bitmap Image { get; }

        Uri ImageUri { get; }
    }
}