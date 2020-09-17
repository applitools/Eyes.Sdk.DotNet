using System.Collections.Generic;

namespace CssParser.Model.Rules
{
    public interface IRuleContainer
    {
        List<RuleSet> Declarations { get; }
    }
}