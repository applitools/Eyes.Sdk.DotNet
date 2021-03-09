using Applitools.Utils;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Diagnostics;

namespace Applitools.Tests.Utils
{
    public abstract class ReportingTestSuite
    {
        private static readonly TestResultReportSummary reportSummary_ = new TestResultReportSummary();
        protected readonly Dictionary<string, object> suiteArgs_ = new Dictionary<string, object>();
        private static readonly HashSet<string> includedTestsList = null;
        private static readonly string excludedTestsListFilename;
        public static readonly bool IS_FULL_COVERAGE = "true".Equals(Environment.GetEnvironmentVariable("APPLITOOLS_FULL_COVERAGE"), StringComparison.OrdinalIgnoreCase);
        public static readonly bool RUNS_ON_CI = Environment.GetEnvironmentVariable("CI") != null;
        public static readonly bool USE_MOCK_VG = "true".Equals(Environment.GetEnvironmentVariable("USE_MOCK_VG"), StringComparison.OrdinalIgnoreCase);
        private readonly static Logger logger_ = new Logger();

        static ReportingTestSuite()
        {
            logger_.SetLogHandler(new NunitLogHandler());
            logger_.Log(TraceLevel.Notice, Stage.TestFramework,
                new
                {
                    TRAVIS_TAG = Environment.GetEnvironmentVariable("TRAVIS_TAG") ?? "<null>",
                    IS_FULL_COVERAGE,
                    SendToSandbox = TestResultReportSummary.SendToSandbox()
                });

            if (!IS_FULL_COVERAGE)
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string asmName = asm.GetName().Name;
                excludedTestsListFilename = "passed_tests_" + asmName + ".txt";

                Stream includedTestsListStream = CommonUtils.ReadResourceStream(asmName + ".Resources.IncludedTests.txt");
                logger_.Log(TraceLevel.Notice, Stage.TestFramework,
                       new
                       {
                           message = "Reading regression list from embedded resource",
                           resourceName = $"{asmName}.Resources.IncludedTests.txt"
                       });
                
                if (includedTestsListStream != null)
                {
                    includedTestsList = new HashSet<string>(CommonUtils.ReadStreamAsLines(includedTestsListStream));
                }
                else
                {
                    includedTestsList = null;
                }

                if (RUNS_ON_CI && excludedTestsListFilename != null && File.Exists(excludedTestsListFilename))
                {
                    logger_.Log(TraceLevel.Notice, Stage.TestFramework,
                        new
                        {
                            message = "Reading exclusion list from file and filtering regression list",
                            excludedTestsListFilename
                        });
                    HashSet<string> excludedTestsList = new HashSet<string>(File.ReadAllLines(excludedTestsListFilename));
                    includedTestsList.RemoveWhere(item => excludedTestsList.Contains(item));
                }
            }
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
        }

        [SetUp]
        public void SetUp()
        {
            TestContext tc = TestContext.CurrentContext;
            if (!IS_FULL_COVERAGE && includedTestsList != null && !includedTestsList.Contains(tc.Test.FullName))
            {
                logger_.Log(TraceLevel.Notice, Stage.TestFramework, StageType.Skipped, new { testName = tc.Test.FullName });
                Assert.Inconclusive();
            }
            else
            {
                logger_.Log(TraceLevel.Notice, Stage.TestFramework, StageType.Start,
                    new { testName = tc.Test.FullName });
            }
        }

        [TearDown]
        public void TearDown()
        {
            string chromedriverPIDs = null;
            if (RUNS_ON_CI) 
            {
                ProcessStartInfo startInfo = new ProcessStartInfo("/bin/bash", "-c \"pgrep chromedriver\"")
                {
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };
                Process p = Process.Start(startInfo);
                chromedriverPIDs = p.StandardOutput.ReadToEnd();
                p.WaitForExit();
            }

            TestContext tc = TestContext.CurrentContext;
            TestStatus status = tc.Result.Outcome.Status;
            logger_.Log(TraceLevel.Notice, Stage.TestFramework, StageType.Complete,
                new { testName = tc.Test.FullName, status, chromedriverPIDs });
            if (status == TestStatus.Inconclusive)
            {
                return;
            }
            if (excludedTestsListFilename != null)
            {
                int attemptsLeft = 3;
                while (attemptsLeft-- > 0 && !WritePassedTestsToFile_(tc, status)) { Thread.Sleep(100); }
            }
            TestResult testResult = GetTestResult();
            reportSummary_.AddResult(testResult);
        }

        private static bool WritePassedTestsToFile_(TestContext tc, TestStatus status)
        {
            try
            {
                File.AppendAllText(excludedTestsListFilename, "");
                if (status == TestStatus.Passed)
                {
                    File.AppendAllLines(excludedTestsListFilename, new string[] { tc.Test.FullName });
                }
                return true;
            }
            catch (Exception ex)
            {
                CommonUtils.LogExceptionStackTrace(logger_, Stage.TestFramework, ex);
                return false;
            }
        }

        protected virtual TestResult GetTestResult()
        {
            TestContext tc = TestContext.CurrentContext;
            TestStatus status = tc.Result.Outcome.Status;
            bool passed = status == TestStatus.Passed;
            TestResult result = new TestResult(tc.Test.MethodName, passed, GetTestParameters());
            return result;
        }

        protected virtual Dictionary<string, object> GetTestParameters()
        {
            Dictionary<string, object> result = new Dictionary<string, object>();
            foreach (KeyValuePair<string, object> suiteArg in suiteArgs_)
            {
                result.Add(suiteArg.Key, suiteArg.Value);
            }
            TestContext tc = TestContext.CurrentContext;
            TestContext.TestAdapter test = tc.Test;
            Type type = Type.GetType(test.ClassName);
            MethodInfo mi = type.GetMethod(test.MethodName);
            ParameterInfo[] pis = mi.GetParameters();
            for (int i = 0; i < pis.Length; ++i)
            {
                string paramName = pis[i].Name;
                object paramValue = test.Arguments[i];
                result.Add(paramName, paramValue);
            }
            return result;
        }

        protected virtual string GetTestName()
        {
            StringBuilder sb = new StringBuilder();
            TestContext.TestAdapter ta = TestContext.CurrentContext.Test;
            sb.Append(ta.MethodName);

            //AppendArguments(sb, suiteArgs_.ToArray(), " [", "]");
            AppendArguments(sb, ta.Arguments, " (", ")");

            return sb.ToString();
        }

        private static void AppendArguments(StringBuilder sb, object[] args, string prefix, string postfix)
        {
            foreach (object arg in args)
            {
                sb.Append(prefix).Append(arg).Append(postfix);
            }
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            //TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: sending json: {JsonConvert.SerializeObject(reportSummary_)}");
            HttpRestClient client = new HttpRestClient(new Uri("http://sdk-test-results.herokuapp.com"));
            client.PostJson("/result", reportSummary_);
        }
    }
}