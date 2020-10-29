using Applitools.Selenium.Tests.Utils;
using Applitools.Tests.Utils;
using Applitools.VisualGrid;
using NUnit.Framework;
using OpenQA.Selenium;
using System;

namespace Applitools.Selenium.Tests.ApiTests
{
    [TestFixture, Parallelizable(ParallelScope.All)]
    public class TestConfigurationSentCorrectlyToServer : ReportingTestSuite
    {
        [TestCase(false, "Test Sequence", "Test Sequence Name Env Var")]
        [TestCase(false, "Test Sequence", null)]
        [TestCase(false, null, "Test Sequence Name Env Var")]
        [TestCase(false, null, null)]
        //[TestCase(true, "Test Sequence", "Test Sequence Name Env Var")]
        //[TestCase(true, "Test Sequence", null)]
        //[TestCase(true, null, "Test Sequence Name Env Var")]
        //[TestCase(true, null, null)]
        public void TestEyesConfiguration(bool useVisualGrid, string sequenceName, string sequenceNameEnvVar)
        {
            EyesRunner runner = useVisualGrid ? (EyesRunner)new VisualGridRunner(10) : new ClassicRunner();
            Eyes eyes = new Eyes(runner);
            TestUtils.SetupLogging(eyes);
            
            IWebDriver driver = SeleniumUtils.CreateChromeDriver();
            driver.Url = "https://applitools.github.io/demo/TestPages/FramesTestPage/";

            string originalBatchSequence = Environment.GetEnvironmentVariable("APPLITOOLS_BATCH_SEQUENCE");
            if (sequenceNameEnvVar != null)
            {
                Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_SEQUENCE", sequenceNameEnvVar);
            }

            string effectiveSequenceName = sequenceName ?? sequenceNameEnvVar;

            BatchInfo batchInfo = new BatchInfo()
            {
                Id = TestDataProvider.BatchInfo.Id + "_" + effectiveSequenceName,
                Name = TestDataProvider.BatchInfo.Name + "_" + effectiveSequenceName
            };

            if (sequenceName != null)
            {
                batchInfo.SequenceName = sequenceName;
            }

            if (sequenceNameEnvVar != null)
            {
                Environment.SetEnvironmentVariable("APPLITOOLS_BATCH_SEQUENCE", originalBatchSequence);
            }

            try
            {
                Assert.AreEqual(effectiveSequenceName, batchInfo.SequenceName, "SequenceName");

                Configuration conf = new Configuration();
                string testName = "Test - " + (useVisualGrid ? "Visual Grid" : "Selenium");
                conf.SetAppName("app").SetTestName(testName)
                    .SetHostApp("someHostApp").SetHostOS("someHostOs")
                    //.SetBaselineBranchName("baseline branch")
                    //.SetBaselineEnvName("baseline env")
                    .SetEnvironmentName("env name")
                    .SetBatch(batchInfo);

                eyes.SetConfiguration(conf);
                eyes.Open(driver);

                eyes.MatchLevel = MatchLevel.Layout;
                eyes.Check(Target.Window());

                eyes.MatchLevel = MatchLevel.Content;
                eyes.Check(Target.Window());
            }
            finally
            {
                driver.Quit();
            }

            TestResults results = eyes.Close(false);
            Metadata.SessionResults sessionResults = TestUtils.GetSessionResults(eyes.ApiKey, results);

            Assert.NotNull(sessionResults, "SessionResults");

            Assert.AreEqual("someHostOs", sessionResults.Env.Os, "OS");
            Assert.AreEqual("someHostApp", sessionResults.Env.HostingApp,"Hosting App");

            Assert.AreEqual(batchInfo.SequenceName, sessionResults.StartInfo.BatchInfo.SequenceName, "Sequence Name");
            //Assert.AreEqual("baseline branch", sessionResults.BaselineBranchName);
            //Assert.AreEqual("baseline env", sessionResults.BaselineEnvId);

            Assert.NotNull(sessionResults.ActualAppOutput, "Actual App Output");
            Assert.AreEqual(2, sessionResults.ActualAppOutput.Length, "Actual App Output");
            Assert.AreEqual(MatchLevel.Layout2, sessionResults.ActualAppOutput[0].ImageMatchSettings.MatchLevel, "Actual App Output (Layout)");
            Assert.AreEqual(MatchLevel.Content, sessionResults.ActualAppOutput[1].ImageMatchSettings.MatchLevel, "Actual App Output (Content)");

            TestResultsSummary resultsSummary = runner.GetAllTestResults(false);
            eyes.Abort();
        }
    }
}
