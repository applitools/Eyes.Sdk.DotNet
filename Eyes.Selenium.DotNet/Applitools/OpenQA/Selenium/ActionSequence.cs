using System.Collections.Generic;

namespace Applitools.OpenQA.Selenium
{
    public interface IActionSequence
    {
        Dictionary<string, object> ToDictionary();
    }
}