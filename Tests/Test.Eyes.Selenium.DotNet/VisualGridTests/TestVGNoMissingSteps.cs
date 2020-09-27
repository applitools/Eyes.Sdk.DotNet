using Applitools.Selenium.VisualGrid;
using Applitools.Tests.Utils;
using Applitools.Ufg;
using Applitools.VisualGrid;
using NUnit.Framework;
using System.Collections.Generic;

namespace Applitools.Selenium.Tests.VisualGridTests
{
    [Parallelizable]
    public class TestVGNoMissingSteps : ReportingTestSuite
    {
        private static Logger logger_ = new Logger();
        private static DesktopBrowserInfo browserInfo_ = new DesktopBrowserInfo(10, 10);

        [Test]
        public void TestNoMissingSteps_UT()
        {
            logger_.SetLogHandler(TestUtils.InitLogHandler());

            Configuration config = new Configuration();
            config.SetTestName(nameof(TestNoMissingSteps_UT));

            List<RunningTest> tests = new List<RunningTest>();

            CreateTest_("test1").AddTask(TaskType.Open).AddTo(tests);
            CreateTest_("test2").SetOpenTaskIssued().AddTo(tests);
            CreateTest_("test3").SetCloseTaskIssued().AddTo(tests);
            CreateTest_("test4").AddTask(TaskType.Open).AddTask(TaskType.Check).AddTo(tests);
            CreateTest_("test5").AddTask(TaskType.Open).AddTask(TaskType.Check).AddTask(TaskType.Close).AddTo(tests);
            CreateTest_("test6").AddTask(TaskType.Open).AddTask(TaskType.Abort).AddTo(tests);
            CreateTest_("test7").SetOpenTaskIssued().SetCloseTaskIssued().AddTo(tests);
            CreateTest_("test8").AddTo(tests);
            CreateTest_("test9").SetOpenTaskIssued().AddTask(TaskType.Check).AddTo(tests);

            List<RunningTest> filteredList = VisualGridEyes.CollectTestsForCheck_(logger_, tests);

            Assert.AreEqual(4, filteredList.Count);
            Assert.AreEqual("test1", filteredList[0].TestName);
            Assert.AreEqual("test2", filteredList[1].TestName);
            Assert.AreEqual("test4", filteredList[2].TestName);
            Assert.AreEqual("test9", filteredList[3].TestName);
        }

        private RunningTestBuilder CreateTest_(string testName)
        {
            RunningTest test = new RunningTest(new RenderBrowserInfo(browserInfo_), logger_);
            test.TestName = testName;
            RunningTestBuilder builder = new RunningTestBuilder(test);
            return builder;
        }

        private class RunningTestBuilder
        {
            private RunningTest runningTest_;
            internal RunningTestBuilder(RunningTest runningTest)
            {
                runningTest_ = runningTest;
            }

            internal void AddTo(List<RunningTest> tests)
            {
                tests.Add(runningTest_);
            }

            internal RunningTestBuilder AddTask(TaskType taskType)
            {
                VisualGridTask task = CreateTask_(taskType);
                runningTest_.TaskList.Add(task);
                return this;
            }

            private VisualGridTask CreateTask_(TaskType taskType)
            {
                return new VisualGridTask(taskType, logger_, runningTest_);
            }

            internal RunningTestBuilder SetOpenTaskIssued()
            {
                runningTest_.openTask_ = CreateTask_(TaskType.Open);
                return this;
            }

            internal RunningTestBuilder SetCloseTaskIssued()
            {
                runningTest_.closeTask_ = CreateTask_(TaskType.Close);
                return this;
            }
        }
    }
}
