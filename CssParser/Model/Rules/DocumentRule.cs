using System.Collections.Generic;
using System.Text;
using CssParser.Model.Extensions;

namespace CssParser.Model.Rules
{
    public sealed class DocumentRule : AggregateRule
    {
        internal DocumentRule()
        { 
            RuleType = RuleType.Document;
            Conditions = new List<Tup<DocumentFunction, string>>();
        }

        public string ConditionText
        {
            get
            {
                var builder = new StringBuilder();
                var concat = false;

                foreach (var condition in Conditions)
                {
                    if (concat)
                    {
                        builder.Append(',');
                    }

                    switch (condition.Item1)
                    {
                        case DocumentFunction.Url:
                            builder.Append("url");
                            break;

                        case DocumentFunction.UrlPrefix:
                            builder.Append("url-prefix");
                            break;

                        case DocumentFunction.Domain:
                            builder.Append("domain");
                            break;

                        case DocumentFunction.RegExp:
                            builder.Append("regexp");
                            break;
                    }

                    builder.Append(Specification.ParenOpen);
                    builder.Append(Specification.DoubleQuote);
                    builder.Append(condition.Item2);
                    builder.Append(Specification.DoubleQuote);
                    builder.Append(Specification.ParenClose);
                    concat = true;
                }

                return builder.ToString();
            }
        }

        internal List<Tup<DocumentFunction, string>> Conditions { get; private set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return "@document " + ConditionText + " {" + 
                RuleSets + 
                "}".NewLineIndent(friendlyFormat, indentation);
        }
    }
}
