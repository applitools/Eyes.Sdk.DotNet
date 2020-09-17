using CssParser.Model.Extensions;

namespace CssParser.Model.Rules
{
    public class ImportRule : RuleSet, ISupportsMedia
    {
        public ImportRule() 
        {
            Media = new MediaTypeList();
            RuleType = RuleType.Import;
        }

        public string Href { get; set; }

        public MediaTypeList Media { get; private set; }

        public override string ToString()
        {
            return ToString(false);
        }

        public override string ToString(bool friendlyFormat, int indentation = 0)
        {
            return Media.Count > 0
                ? string.Format("@import url({0}) {1};", Href, Media).NewLineIndent(friendlyFormat, indentation)
                : string.Format("@import url({0});", Href).NewLineIndent(friendlyFormat, indentation);
        }
    }
}
