using Applitools.Fluent;
using System.Drawing;

namespace Applitools
{
    public delegate AppOutput AppOutputProviderDelegate(Rectangle? region, ICheckSettingsInternal checkSettingsInternal, ImageMatchSettings imageMatchSettings);
}