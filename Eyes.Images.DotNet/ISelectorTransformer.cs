using Applitools.Fluent;
using OpenQA.Selenium;

namespace Applitools
{
    public interface ISelectorTransformer
    {
        RegionSelector GetRegionSelector(By selector);
    }
}