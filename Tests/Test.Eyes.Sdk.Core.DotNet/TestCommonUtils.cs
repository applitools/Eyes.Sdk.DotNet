using Applitools.Tests.Utils;
using Applitools.Utils;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using NUnit.Framework;
using System.IO;

namespace Applitools.Utils
{
    [TestFixture]
    [Parallelizable(ParallelScope.All)]
    public class TestCommonUtils : ReportingTestSuite
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

        private class JobInfo
        {
            public string Renderer { get; set; } = "";
            public object EyesEnvironment { get; set; } = "";
        }

        //[Test]
        public void TestDynamicDeserialization()
        {
            string json = "[{\"renderer\":\"chrome-0\",\"eyesEnvironment\":{\"os\":\"Linux\",\"osInfo\":\"Linux\",\"hostingApp\":\"Chrome 86.0\",\"hostingAppInfo\":\"Chrome 86.0\",\"deviceInfo\":\"Desktop\",\"displaySize\":{\"width\":1024,\"height\":768},\"0.wxk0hymm03q\":\"got you!\"}}]";
            JObject[] jobInfosUnparsed = JsonConvert.DeserializeObject<JObject[]>(json);
            JobInfo[] jobInfos = new JobInfo[jobInfosUnparsed.Length];
            for (int i = 0; i < jobInfos.Length; ++i)
            {
                JObject jobInfoUnparsed = jobInfosUnparsed[i];
                JobInfo jobInfo = new JobInfo();
                jobInfo.Renderer = jobInfoUnparsed.Value<string>("renderer");
                jobInfo.EyesEnvironment = jobInfoUnparsed.Value<object>("eyesEnvironment");
                jobInfos[i] = jobInfo;
            }

            string outputJson = JsonConvert.SerializeObject(jobInfos);
        }
    }
}
