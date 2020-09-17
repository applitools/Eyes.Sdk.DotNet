using Applitools.Utils;
using NUnit.Framework;

namespace Applitools
{
    [TestFixture]
    public class TestStringUtils
    {
        [TestCase(null, null, true)]
        [TestCase("   \t  ", null, true)]
        [TestCase(null, "   \n\n  ", true)]
        [TestCase("     ", "\n\n        \t\t\n\n\t\t\t\n    ", true)]
        [TestCase("\n\n        \t\t\n\n\t\t\t\n    ", "    \t\n ", true)]
        [TestCase("\n\n        \t\t\n\n\t\t\t\n    ", "   asdf \t\n ", false)]
        [TestCase("\n\n        \t\tas\nd\n\tf\t\t\n    ", "   asdf \t\n ", true)]
        [TestCase("\n\n     asdf   \t\t\n\n\t\t\t\n    ", "   asdf \t\n ", true)]
        [TestCase("\n\n     asdf   \t\t\n\n\t\t\t\n    g", "   asdf \t\n ", false)]
        [TestCase("\n\n     asdf   \t\t\n\n\t\t\t\n    g", "   asdf \t\n h", false)]
        [TestCase("\n\n     asdf   \t\t\n\n\t\t\t\n    g", "   asdf \t\n g", true)]
        [TestCase("\n\n     asdf   \t\t\n\n\t\t\t\n    g", "   asdf \t\n gg", false)]
        [TestCase("asdf", "   as\td\nf ", true)]
        [TestCase("asdf", "asdf ", true)]
        [TestCase("asdf", "ASDF ", false)]
        public void TestStringEqualsIgnoreWhiteSpace(string strX, string strY, bool areEqual)
        {
            Assert.AreEqual(areEqual, StringUtils.StringEqualsIgnoreWhiteSpace(strX, strY));
        }

        [Test]
        public void TestStringSplitToMaximumLength()
        {
            CollectionAssert.AreEqual(new string[] { "AAA", "BBB", "CCC" }, "AAABBBCCC".Split(3));
            CollectionAssert.AreEqual(new string[] { "AAA", "BBB", "CC" }, "AAABBBCC".Split(3));
            CollectionAssert.AreEqual(new string[] { "AAA", "BBB", "C" }, "AAABBBC".Split(3));
        }
    }
}
