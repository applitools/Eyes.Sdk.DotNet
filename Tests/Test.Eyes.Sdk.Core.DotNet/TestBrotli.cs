using Applitools.Tests.Utils;
using Applitools.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Reflection;

namespace Applitools.Tests
{
    public class TestBrotli : ReportingTestSuite
    {
        private static Assembly thisAsm_ = Assembly.GetExecutingAssembly();

        public static string[][] TestData
        {
            get
            {
                string[] allResourceNames = thisAsm_.GetManifestResourceNames();
                string[] brotliResourceNames = allResourceNames.Where(x => x.StartsWith("Test.Eyes.Sdk.Core.DotNet.Resources.BrotliTestData")).ToArray();
                string[] brotliUncompressedResourceNames = brotliResourceNames.Where(x => !(x.EndsWith(".compressed") || x.Contains(".compressed."))).ToArray();
                string[] brotliCompressedResourceNames = brotliResourceNames.Where(x => x.EndsWith(".compressed") || x.Contains(".compressed.")).ToArray();
                List<string[]> data = new List<string[]>();
                foreach (string uncompressedName in brotliUncompressedResourceNames)
                {
                    foreach (string compressedName in brotliCompressedResourceNames.Where(x => x.StartsWith(uncompressedName + ".compressed")))
                    {
                        data.Add(new string[] { uncompressedName, compressedName });
                    }
                }
                return data.ToArray();
            }
        }

        [TestCaseSource(nameof(TestData))]
        public void TestBrotliDecompression(string expected, string compressed)
        {
            Stream expectedStream = thisAsm_.GetManifestResourceStream(expected);
            byte[] expectedBytes = CommonUtils.ReadToEnd(expectedStream);
            Stream compressedStream = thisAsm_.GetManifestResourceStream(compressed);
            using (Stream decStream = new BrotliStream(compressedStream, CompressionMode.Decompress))
            {
                byte[] decBytes = CommonUtils.ReadToEnd(decStream);
                CollectionAssert.AreEqual(expectedBytes, decBytes, "extracted data different than expected");
            }
        }
    }
}
