using CssParser.Model.Rules;

namespace CssParser.Model
{
    interface ISupportsDeclarations
    {
        StyleDeclaration Declarations { get; }
    }
}