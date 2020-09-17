using System.Drawing;

namespace Applitools.Utils.Cropping
{
    public interface ICutProvider
    {
        /// <summary>
        /// Crops the given image.
        /// </summary>
        /// <param name="image">The image to crop.</param>
        /// <returns>A cropped image.</returns>
        Bitmap Cut(Bitmap image);

        /// <summary>
        /// Get a scaled version of the cut provider.
        /// </summary>
        /// <param name="scaleRatio">The ratio by which to scale the current cut parameters.</param>
        /// <returns>A new scale cut provider instance.</returns>
        ICutProvider Scale(double scaleRatio);

        Rectangle ToRectangle(Size size);
    }
}
