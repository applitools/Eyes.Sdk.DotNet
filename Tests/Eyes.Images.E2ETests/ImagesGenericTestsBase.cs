using System;
using Applitools;
using NUnit.Framework;

namespace Eyes.Images.E2ETests
{
    public abstract class ImagesGenericTestsBase
    {
        protected Applitools.Images.Eyes eyes;
        private static string TEST_SUITE_NAME = "Eyes Image SDK";
        protected static BatchInfo Batch = new BatchInfo(TEST_SUITE_NAME);

        [SetUp]
        public void Setup()
        {
            eyes = new Applitools.Images.Eyes
            {
                ApiKey = Environment.GetEnvironmentVariable("APPLITOOLS_API_KEY"), 
                BranchName = "master_java",
                //eyes.Logger = new Logger();
                //LogHandler logHandler = new StdoutLogHandler(verboseLogs);
                SaveNewTests = true,
                Batch = Batch
            };

            if (Environment.GetEnvironmentVariable("APPLITOOLS_USE_PROXY") != null)
            {
                eyes.Proxy = new ProxySettings("http://127.0.0.1", 8888);
            }
        }

        [TearDown]
        public void TearDown()
        {
            eyes.AbortIfNotClosed();
        }

        protected string GetApplicationName()
        {
            return "Applitools Eyes SDK";
        }
    }
}