using System.Drawing;

namespace Applitools.Common
{
    public static class Target
    {
        public static ICheckSettings Window()
        {
            return new CheckSettings();
        }

        public static ICheckSettings Region(Rectangle rect)
        {
            return new CheckSettings();
        }
    }
}
