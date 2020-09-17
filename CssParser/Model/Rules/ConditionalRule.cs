

namespace CssParser.Model.Rules
{
    public abstract class ConditionalRule : AggregateRule
    {
        public virtual string Condition
        {
            get { return string.Empty; }
            set { }
        }
    }
}
