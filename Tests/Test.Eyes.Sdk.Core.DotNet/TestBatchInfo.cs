using System;
using Applitools.Tests.Utils;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Applitools
{
    [TestFixture]
    public class TestBatchInfo : ReportingTestSuite
    {
        [Test]
        public void TestCtor()
        {
            BatchInfo bi = new BatchInfo("batch info test");
            Assert.AreEqual("batch info test", bi.Name);
            Assert.NotNull(bi.StartedAt);
        }

        [Test]
        public void TestBatchInfoDate()
        {
            var dto = new DateTimeOffset(1, 2, 3, 4, 5, 6, 7, TimeSpan.FromHours(8));
            BatchInfo bi = new BatchInfo("test", dto);
            Assert.AreEqual("test", bi.Name);
            Assert.AreEqual(dto, bi.StartedAt);
        }

        [Test]
        public void TestToString()
        {
            BatchInfo bi = new BatchInfo(
                "test", new DateTimeOffset(1, 2, 3, 4, 5, 6, TimeSpan.FromHours(1)));
            bi.Id = "5023-1d";
            bi.SequenceName = "seq A";
            Assert.AreEqual("test [5023-1d] <seq A> - 0001-02-03T04:05:06+01:00", bi.ToString());
        }

        [Test]
        public void TestDeserialization()
        {
            string json = "{\"id\": \"abc-def\",\"name\": \"SomeName\",\"isImplicit\": false,\"startedAt\": \"2021-03-18T15:37:00Z\",\"lastRunAt\": \"2021-03-18T15:37:04.8603032Z\",\"duration\": 0,\"notifyOnCompletion\": false,\"properties\": [{\"name\": \"some\", \"value\":\"thing\"}]}";
            BatchInfo batchInfo = JsonConvert.DeserializeObject<BatchInfo>(json);
            Assert.AreEqual("abc-def", batchInfo.Id);
            Assert.AreEqual("SomeName", batchInfo.Name);
            Assert.AreEqual(new DateTimeOffset(2021, 3, 18, 15, 37, 0, TimeSpan.Zero), batchInfo.StartedAt);
            Assert.AreEqual(false, batchInfo.NotifyOnCompletion);

            PropertiesCollection props = new PropertiesCollection();
            props.Add("some", "thing");
            CollectionAssert.AreEquivalent(props, batchInfo.Properties);
        }
    }
}
