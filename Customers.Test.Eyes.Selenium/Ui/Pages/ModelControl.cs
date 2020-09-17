namespace Applitools.Ui.Pages
{
    using System.Drawing;
    using Applitools.Utils;

    /// <summary>
    /// A model window control
    /// </summary>
    public class ModelControl
    {
        public ModelControl(int centerX, int centerY)
        {
            ArgumentGuard.GreaterOrEqual(centerX, 0, nameof(centerX));
            ArgumentGuard.GreaterOrEqual(centerY, 0, nameof(centerY));

            CenterX = centerX;
            CenterY = centerY;
        }

        public int CenterX { get; }

        public int CenterY { get; }

        public Point Center
        {
            get { return new Point(CenterX, CenterY); }
        }
    }
}
