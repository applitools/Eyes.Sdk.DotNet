using System.Collections.Generic;

namespace Applitools.OpenQA.Selenium
{
    public interface IActionExecutor
    {
        void PerformActions(IList<IActionSequence> actionSequenceList);
    }
}