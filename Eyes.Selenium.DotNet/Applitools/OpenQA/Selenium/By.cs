using System.Runtime.CompilerServices;

namespace Applitools.OpenQA.Selenium
{
    public class By
    {
        public string Mechanism { get; } = string.Empty;

        public string Criteria { get; } = string.Empty;

        protected By(string criteria, [CallerMemberName] string mechanism = null)
        {
            Mechanism = mechanism;
            Criteria = criteria;
        }

        public static By Id(string idToFind) => new By(idToFind);

        public static By LinkText(string linkTextToFind) => new By(linkTextToFind);

        public static By Name(string nameToFind) => new By(nameToFind);

        public static By XPath(string xpathToFind) => new By(xpathToFind);

        public static By ClassName(string classNameToFind) => new By(classNameToFind);

        public static By PartialLinkText(string partialLinkTextToFind) => new By(partialLinkTextToFind);

        public static By TagName(string tagNameToFind) => new By(tagNameToFind);

        public static By CssSelector(string cssSelectorToFind) => new By(cssSelectorToFind);
    }
}
