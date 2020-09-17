using Applitools.Fluent;
using System.Drawing;

namespace Applitools
{
    public delegate AppOutputWithScreenshot AppOutputProviderDelegate(Rectangle? region, ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings);
}