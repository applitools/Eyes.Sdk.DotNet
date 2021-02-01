using Applitools.Utils;
using Newtonsoft.Json;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;

namespace Applitools.Tests.Utils
{
    public abstract class ReportingTestSuite
    {
        private static readonly TestResultReportSummary reportSummary_ = new TestResultReportSummary();
        protected readonly Dictionary<string, object> suiteArgs_ = new Dictionary<string, object>();
        private static readonly IList<string> includedTestsList = null;
        private static readonly string includedTestsListFilename;
        public static readonly bool IS_FULL_COVERAGE = "true".Equals(Environment.GetEnvironmentVariable("APPLITOOLS_FULL_COVERAGE"), StringComparison.OrdinalIgnoreCase);
        public static readonly bool RUNS_ON_CI = Environment.GetEnvironmentVariable("CI") != null;
        public static readonly bool USE_MOCK_VG = "true".Equals(Environment.GetEnvironmentVariable("USE_MOCK_VG"), StringComparison.OrdinalIgnoreCase);
        static ReportingTestSuite()
        {
            TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: TRAVIS_TAG: '{Environment.GetEnvironmentVariable("TRAVIS_TAG") ?? "<null>"}'");
            TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: IS_FULL_COVERAGE: {IS_FULL_COVERAGE}");
            TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Send to sandbox: {TestResultReportSummary.SendToSandbox()}");
            if (!IS_FULL_COVERAGE)
            {
                Assembly asm = Assembly.GetExecutingAssembly();
                string asmName = asm.GetName().Name;
                includedTestsListFilename = "failed_tests_" + asmName + ".txt";
                if (!RUNS_ON_CI || includedTestsListFilename == null || !File.Exists(includedTestsListFilename))
                {
                    TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Reading regression list from embedded resource: {asmName}.Resources.IncludedTests.txt");
                    Stream includedTestsListStream = CommonUtils.ReadResourceStream(asmName + ".Resources.IncludedTests.txt");
                    if (includedTestsListStream != null)
                    {
                        includedTestsList = CommonUtils.ReadStreamAsLines(includedTestsListStream);
                    }
                    else
                    {
                        includedTestsList = null;
                    }
                }
                else
                {
                    TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Reading regression list from file: {includedTestsListFilename}");
                    includedTestsList = File.ReadAllLines(includedTestsListFilename);
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
                TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Skipping Test (will be marked as Inconclusive): {tc.Test.FullName}");
                Assert.Inconclusive();
            }
            else
            {
                TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Test Starting: {tc.Test.FullName}");
            }
        }

        [TearDown]
        public void TearDown()
        {
            TestContext tc = TestContext.CurrentContext;
            TestStatus status = tc.Result.Outcome.Status;
            TestContext.Progress.WriteLine($"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Test {status}: {tc.Test.FullName}");
            if (status == TestStatus.Inconclusive)
            {
                return;
            }
            if (includedTestsListFilename != null)
            {
                int attemptsLeft = 3;
                while (attemptsLeft-- > 0 && !WriteFailedTestsToFile_(tc, status)) { Thread.Sleep(100); }
            }
            TestResult testResult = GetTestResult();
            reportSummary_.AddResult(testResult);
        }

        private static bool WriteFailedTestsToFile_(TestContext tc, TestStatus status)
        {
            try
            {
                File.AppendAllText(includedTestsListFilename, "");
                if (status != TestStatus.Passed)
                {
                    File.AppendAllLines(includedTestsListFilename, new string[] { tc.Test.FullName });
                }
                return true;
            }
            catch (Exception ex)
            {
                string s = $"{DateTimeOffset.Now:yyyy'-'MM'-'dd HH':'mm':'ss.fff} - Eyes: Test Report Error: {ex}";
                TestContext.Progress.WriteLine(s);
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