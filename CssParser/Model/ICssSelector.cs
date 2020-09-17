using CssParser.Model.Selector;

namespace CssParser.Model
{
    interface ISupportsSelector
    {
        BaseSelector Selector { get; set; }
    }
}