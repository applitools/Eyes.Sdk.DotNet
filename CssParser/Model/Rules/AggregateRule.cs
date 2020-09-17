using System.Collections.Generic;

namespace CssParser.Model.Rules
{
    public abstract class AggregateRule : RuleSet, ISupportsRuleSets
    {
        protected AggregateRule()
        {
            RuleSets = new List<RuleSet>();
        }

        public List<RuleSet> RuleSets { get; private set; }
    }
}
