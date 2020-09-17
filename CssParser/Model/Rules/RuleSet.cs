namespace CssParser.Model.Rules
{
    public abstract class RuleSet
    {
        internal RuleSet()
        {
            RuleType = RuleType.Unknown;
        }

        public RuleType RuleType { get; set; }

        public abstract string ToString(bool friendlyFormat, int indentation = 0);
    }
}
