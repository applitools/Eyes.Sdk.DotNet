using Applitools.Tests.Utils;
using NUnit.Framework;
using System;
using System.Collections.Generic;

namespace Applitools.Tests
{
    [TestFixture]
    public class TestEnvironmentVariables : ReportingTestSuite
    {
        private readonly Logger logger_ = new Logger();

        private Dictionary<string, string> originalValues_ = new Dictionary<string, string>();


        [OneTimeSetUp]
        public void CollectOriginalValues()
        {
            GetEnvVar_("APPLITOOLS_API_KEY");
            GetEnvVar_("APPLITOOLS_SERVER_URL");
            GetEnvVar_("APPLITOOLS_BATCH_ID");
            GetEnvVar_("APPLITOOLS_BATCH_NAME");
            GetEnvVar_("APPLITOOLS_BATCH_SEQUENCE");
            GetEnvVar_("APPLITOOLS_BATCH_NOTIFY");
            GetEnvVar_("APPLITOOLS_BRANCH");
            GetEnvVar_("APPLITOOLS_PARENT_BRANCH");
            GetEnvVar_("APPLITOOLS_BASELINE_BRANCH");
            GetEnvVar_("APPLITOOLS_DONT_CLOSE_BATCHES");
        }

        private void GetEnvVar_(string envVarName)
        {
            originalValues_[envVarName] = Environment.GetEnvironmentVariable(envVarName);
            originalValues_["bamboo_" + envVarName] = Environment.GetEnvironmentVariable("bamboo_" + envVarName);
        }

        [OneTimeTearDown]
        public void ReturnOriginalValues()
        {
            foreach (KeyValuePair<string, string> kvp in originalValues_)
            {
                Environment.SetEnvironmentVariable(kvp.Key, kvp.Value);
            }
        }

        [SetUp]
        public void ResetEnvVars()
        {
            ResetAllEnvVars_();
        }

        [Test]
        public void TestApiKeyEnvironmentVariables()
        {
            TestEyes testEyes = new TestEyes();
            IServerConnector serverConnector = testEyes.ServerConnector;

            Environment.SetEnvironmentVariable("APPLITOOLS_API_KEY", "ApiKeyTest1234");
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("ApiKeyTest1234", serverConnector.ApiKey, nameof(serverConnector.ApiKey));
            
            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_API_KEY", "bambooApiKeyTest1234");
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("ApiKeyTest1234", serverConnector.ApiKey, nameof(serverConnector.ApiKey));
            
            Environment.SetEnvironmentVariable("APPLITOOLS_API_KEY", null);
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("bambooApiKeyTest1234", serverConnector.ApiKey, nameof(serverConnector.ApiKey));
        }

        [Test]
        public void TestServerUrlEnvironmentVariables()
        {
            TestEyes testEyes = new TestEyes();
            IServerConnector serverConnector = testEyes.ServerConnector;
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("https://eyesapi.applitools.com/", serverConnector.ServerUrl.AbsoluteUri, nameof(serverConnector.ServerUrl));

            Environment.SetEnvironmentVariable("APPLITOOLS_SERVER_URL", "https://some.testurl.com/");
            testEyes = new TestEyes();
            serverConnector = testEyes.ServerConnector;
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("https://some.testurl.com/", serverConnector.ServerUrl.AbsoluteUri, nameof(serverConnector.ServerUrl));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_SERVER_URL", "https://bamboo.testurl.com/");
            testEyes = new TestEyes();
            serverConnector = testEyes.ServerConnector;
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("https://some.testurl.com/", serverConnector.ServerUrl.AbsoluteUri, nameof(serverConnector.ServerUrl));

            Environment.SetEnvironmentVariable("APPLITOOLS_SERVER_URL", null);
            testEyes = new TestEyes();
            serverConnector = testEyes.ServerConnector;
            testEyes.UpdateServerConnector_(); // call this instead of calling eyes.OpenBase.
            Assert.AreEqual("https://bamboo.testurl.com/", serverConnector.ServerUrl.AbsoluteUri, nameof(serverConnector.ServerUrl));
        }

        [Test]
        public void TestDontCloseBatchesEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_DONT_CLOSE_BATCHES", "true");
            ServerConnector serverConnector = new ServerConnector(logger_);
            Assert.AreEqual(true, serverConnector.DontCloseBatches, nameof(serverConnector.DontCloseBatches));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_DONT_CLOSE_BATCHES", "false");
            serverConnector = new ServerConnector(logger_);
            Assert.AreEqual(true, serverConnector.DontCloseBatches, nameof(serverConnector.DontCloseBatches));

            Environment.SetEnvironmentVariable("APPLITOOLS_DONT_CLOSE_BATCHES", null);
            serverConnector = new ServerConnector(logger_);
            Assert.AreEqual(false, serverConnector.DontCloseBatches, nameof(serverConnector.DontCloseBatches));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_DONT_CLOSE_BATCHES", "true");
            serverConnector = new ServerConnector(logger_);
            Assert.AreEqual(true, serverConnector.DontCloseBatches, nameof(serverConnector.DontCloseBatches));
        }

        [Test]
        public void TestBatchIdEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_ID", "testBatchId");
            BatchInfo batchInfo = new BatchInfo();
            Assert.AreEqual("testBatchId", batchInfo.Id, nameof(batchInfo.Id));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BATCH_ID", "bambooTestBatchId");
            batchInfo = new BatchInfo();
            Assert.AreEqual("testBatchId", batchInfo.Id, nameof(batchInfo.Id));

            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_ID", null);
            batchInfo = new BatchInfo();
            Assert.AreEqual("bambooTestBatchId", batchInfo.Id, nameof(batchInfo.Id));
        }

        [Test]
        public void TestBatchNameEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_NAME", "testBatchName");
            BatchInfo batchInfo = new BatchInfo();
            Assert.AreEqual("testBatchName", batchInfo.Name, nameof(batchInfo.Name));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BATCH_NAME", "bambooTestBatchName");
            batchInfo = new BatchInfo();
            Assert.AreEqual("testBatchName", batchInfo.Name, nameof(batchInfo.Name));

            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_NAME", null);
            batchInfo = new BatchInfo();
            Assert.AreEqual("bambooTestBatchName", batchInfo.Name, nameof(batchInfo.Name));
        }

        [Test]
        public void TestBatchSequenceNameEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_SEQUENCE", "testBatchSequence");
            BatchInfo batchInfo = new BatchInfo();
            Assert.AreEqual("testBatchSequence", batchInfo.SequenceName, nameof(batchInfo.SequenceName));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BATCH_SEQUENCE", "bambooTestBatchSequence");
            batchInfo = new BatchInfo();
            Assert.AreEqual("testBatchSequence", batchInfo.SequenceName, nameof(batchInfo.SequenceName));

            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_SEQUENCE", null);
            batchInfo = new BatchInfo();
            Assert.AreEqual("bambooTestBatchSequence", batchInfo.SequenceName, nameof(batchInfo.SequenceName));
        }

        [Test]
        public void TestBatchNotifyEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_NOTIFY", "true");
            BatchInfo batchInfo = new BatchInfo();
            Assert.AreEqual(true, batchInfo.NotifyOnCompletion, nameof(batchInfo.NotifyOnCompletion));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BATCH_NOTIFY", "false");
            batchInfo = new BatchInfo();
            Assert.AreEqual(true, batchInfo.NotifyOnCompletion, nameof(batchInfo.NotifyOnCompletion));

            Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_NOTIFY", null);
            batchInfo = new BatchInfo();
            Assert.AreEqual(false, batchInfo.NotifyOnCompletion, nameof(batchInfo.NotifyOnCompletion));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BATCH_NOTIFY", "true");
            batchInfo = new BatchInfo();
            Assert.AreEqual(true, batchInfo.NotifyOnCompletion, nameof(batchInfo.NotifyOnCompletion));
        }

        [Test]
        public void TestBranchEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_BRANCH", "testBranchName");
            Configuration config = new Configuration();
            Assert.AreEqual("testBranchName", config.BranchName, nameof(config.BranchName));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BRANCH", "bambooTestBranchName");
            config = new Configuration();
            Assert.AreEqual("testBranchName", config.BranchName, nameof(config.BranchName));

            Environment.SetEnvironmentVariable("APPLITOOLS_BRANCH", null);
            config = new Configuration();
            Assert.AreEqual("bambooTestBranchName", config.BranchName, nameof(config.BranchName));
        }

        [Test]
        public void TestParentBranchEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_PARENT_BRANCH", "testParentBranchName");
            Configuration config = new Configuration();
            Assert.AreEqual("testParentBranchName", config.ParentBranchName, nameof(config.ParentBranchName));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_PARENT_BRANCH", "bambooParentTestBranchName");
            config = new Configuration();
            Assert.AreEqual("testParentBranchName", config.ParentBranchName, nameof(config.ParentBranchName));

            Environment.SetEnvironmentVariable("APPLITOOLS_PARENT_BRANCH", null);
            config = new Configuration();
            Assert.AreEqual("bambooParentTestBranchName", config.ParentBranchName, nameof(config.ParentBranchName));
        }

        [Test]
        public void TestBaselineBranchEnvironmentVariables()
        {
            Environment.SetEnvironmentVariable("APPLITOOLS_BASELINE_BRANCH", "testBaselineBranchName");
            Configuration config = new Configuration();
            Assert.AreEqual("testBaselineBranchName", config.BaselineBranchName, nameof(config.BaselineBranchName));

            Environment.SetEnvironmentVariable("bamboo_APPLITOOLS_BASELINE_BRANCH", "bambooBaselineTestBranchName");
            config = new Configuration();
            Assert.AreEqual("testBaselineBranchName", config.BaselineBranchName, nameof(config.BaselineBranchName));

            Environment.SetEnvironmentVariable("APPLITOOLS_BASELINE_BRANCH", null);
            config = new Configuration();
            Assert.AreEqual("bambooBaselineTestBranchName", config.BaselineBranchName, nameof(config.BaselineBranchName));
        }

        private void ResetAllEnvVars_()
        {
            SetEnvVar_("APPLITOOLS_API_KEY", null);
            SetEnvVar_("APPLITOOLS_SERVER_URL", null);
            SetEnvVar_("APPLITOOLS_BATCH_ID", null);
            SetEnvVar_("APPLITOOLS_BATCH_NAME", null);
            SetEnvVar_("APPLITOOLS_BATCH_SEQUENCE", null);
            SetEnvVar_("APPLITOOLS_BATCH_NOTIFY", null);
            SetEnvVar_("APPLITOOLS_BRANCH", null);
            SetEnvVar_("APPLITOOLS_PARENT_BRANCH", null);
            SetEnvVar_("APPLITOOLS_BASELINE_BRANCH", null);
            SetEnvVar_("APPLITOOLS_DONT_CLOSE_BATCHES", null);
        }

        private void SetEnvVar_(string envVarName, string envVarValue)
        {
            Environment.SetEnvironmentVariable(envVarName, envVarValue);
            Environment.SetEnvironmentVariable("bamboo_" + envVarName, envVarValue);
        }
    }
}
