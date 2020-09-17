using Applitools.Utils;
using NUnit.Framework;
using System.Collections.Generic;

namespace Applitools
{
    [TestFixture]
    public class TestStringReplacement
    {

        public static IEnumerable<TestCaseData> TestStringData
        {
            get
            {
                yield return new TestCaseData("@<",">#", "abcdef@<0>#ghijklmnop@<1>#qrstuv@<2>#wx@<1>#@<0>#yz",
                                            new Dictionary<string, string> { { "0", "ABCDEFG" }, { "1", "HIJKLMNOP" }, { "2", "QRSTUV" }, { "3", "WXYZ" } },
                                            "abcdefABCDEFGghijklmnopHIJKLMNOPqrstuvQRSTUVwxHIJKLMNOPABCDEFGyz");
            }
        }

        [Test]
        [TestCaseSource("TestStringData")]
        public void EfficientStringReplace(string refIdOpenToken, string refIdCloseToken, string input, IDictionary<string, string> replacements, string expectedResult)
        {
            string result = StringUtils.EfficientStringReplace(refIdOpenToken, refIdCloseToken, input, replacements);
            Assert.AreEqual(expectedResult, result);
        }
    }
}
