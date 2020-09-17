namespace Applitools
{
    using NUnit.Framework;
    using System;

    [TestFixture]
    public class TestTestResults
    {
        [Test]
        public void TestTestResultsObject()
        {
            TestResults tr = new TestResults();
            Assert.AreEqual(0, tr.ContentMatches);
            Assert.AreEqual(0, tr.ExactMatches);
            Assert.AreEqual(0, tr.LayoutMatches);
            Assert.AreEqual(0, tr.Matches);
            Assert.AreEqual(0, tr.Mismatches);
            Assert.AreEqual(0, tr.Missing);
            Assert.AreEqual(0, tr.NoneMatches);
            Assert.AreEqual(0, tr.Steps);
            Assert.AreEqual(0, tr.StrictMatches);
            Assert.AreEqual(0, tr.New);
            Assert.AreEqual(TestResultsStatus.Passed, tr.Status);
            Assert.AreEqual(null, tr.ApiUrls);
            Assert.AreEqual(null, tr.AppUrls);
            Assert.AreEqual(null, tr.AppName);
            Assert.AreEqual(null, tr.BaselineId);
            Assert.AreEqual(null, tr.BatchId);
            Assert.AreEqual(null, tr.BatchName);
            Assert.AreEqual(null, tr.BranchName);
            Assert.AreEqual(null, tr.DefaultMatchSettings);
            Assert.AreEqual(0, tr.Duration);
            Assert.AreEqual(null, tr.HostApp);
            Assert.AreEqual(null, tr.HostOS);
            Assert.AreEqual(null, tr.HostDisplaySize);
            Assert.AreEqual(null, tr.Id);
            Assert.AreEqual(false, tr.IsAborted);
            Assert.AreEqual(false, tr.IsDifferent);
            Assert.AreEqual(false, tr.IsNew);
            Assert.AreEqual(true, tr.IsPassed);
            Assert.AreEqual(null, tr.Name);
            Assert.AreEqual(null, tr.SecretToken);
            Assert.AreEqual(default(DateTime), tr.StartedAt);
            Assert.AreEqual(null, tr.StepsInfo);
            Assert.AreEqual(null, tr.AccessibilityStatus);


            tr.ContentMatches = 1;
            tr.ExactMatches = 2;
            tr.LayoutMatches = 3;
            tr.Matches = 4;
            tr.Mismatches = 5;
            tr.Missing = 6;
            tr.NoneMatches = 7;
            tr.Steps = 8;
            tr.StrictMatches = 9;
            tr.New = 10;
            tr.Status = TestResultsStatus.Failed;
            tr.AccessibilityStatus = new SessionAccessibilityStatus() {
                Level = AccessibilityLevel.AAA, 
                Version = AccessibilityGuidelinesVersion.WCAG_2_1,
                Status = AccessibilityStatus.Passed };

            Assert.AreEqual(1, tr.ContentMatches);
            Assert.AreEqual(2, tr.ExactMatches);
            Assert.AreEqual(3, tr.LayoutMatches);
            Assert.AreEqual(4, tr.Matches);
            Assert.AreEqual(5, tr.Mismatches);
            Assert.AreEqual(6, tr.Missing);
            Assert.AreEqual(7, tr.NoneMatches);
            Assert.AreEqual(8, tr.Steps);
            Assert.AreEqual(9, tr.StrictMatches);
            Assert.AreEqual(10, tr.New);
            Assert.AreEqual(TestResultsStatus.Failed, tr.Status);

            Assert.NotNull(tr.AccessibilityStatus);
            Assert.AreEqual(AccessibilityLevel.AAA, tr.AccessibilityStatus.Level);
            Assert.AreEqual(AccessibilityGuidelinesVersion.WCAG_2_1, tr.AccessibilityStatus.Version);
            Assert.AreEqual(AccessibilityStatus.Passed, tr.AccessibilityStatus.Status);
        }
    }
}
