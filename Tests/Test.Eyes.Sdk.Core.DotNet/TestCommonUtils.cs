using Applitools.Utils;
using NUnit.Framework;
using System.IO;

namespace Test.Eyes.Sdk.DotNet.Core
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestCommonUtils
    {
        [Test]
        public void TestReadToEnd()
        {
            byte[] originalData = CommonUtils.ReadResourceBytes("Test.Eyes.Sdk.Core.DotNet.Resources.fa-regular-400.svg");
            using (Stream input = new MemoryStream(originalData))
            {
                byte[] output = input.ReadToEnd();
                Assert.AreEqual(input.Length, output.Length, "length");
                CollectionAssert.AreEqual(originalData, output);
            }
        }

        [TestCase(50)]
        [TestCase(3000)]
        public void TestReadToEndWithMaximumLength(int maxLength)
        {
            byte[] originalData = CommonUtils.ReadResourceBytes("Test.Eyes.Sdk.Core.DotNet.Resources.fa-regular-400.svg");
            using (Stream input = new MemoryStream(originalData))
            {
                byte[] output = input.ReadToEnd(maxLength: maxLength);
                Assert.AreEqual(maxLength, output.Length, "length");
                byte[] expectedData = new byte[maxLength];
                for (int i = 0; i < maxLength; ++i)
                {
                    expectedData[i] = originalData[i];
                }
                CollectionAssert.AreEqual(expectedData, output);
            }
        }
    }
}
