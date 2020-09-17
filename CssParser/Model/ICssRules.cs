using System.Collections.Generic;
using CssParser.Model.Rules;

namespace CssParser.Model
{
    interface ISupportsRuleSets
    {
        List<RuleSet> RuleSets { get; }
    }
}