namespace Applitools.Selenium.Tests
{
    using NUnit.Framework;
    using OpenQA.Selenium;
    using System.Drawing;

    [TestFixture]
    [TestFixtureSource(typeof(TestDataProvider), nameof(TestDataProvider.FixtureArgs))]
    [Parallelizable]
    public class TestAcme : TestSetup
    {
        public TestAcme(DriverOptions options) : this(options, false) { }

        public TestAcme(DriverOptions options, bool useVisualGrid)
            : base("Eyes Selenium SDK - ACME", options, useVisualGrid)
        {
            testedPageSize = new Size(1024, 768);
        }

        public TestAcme(DriverOptions options, StitchModes stitchMode)
            : base("Eyes Selenium SDK - ACME", options, stitchMode)
        {
            testedPageSize = new Size(1024, 768);
        }

        //[Test, Ignore("doesn't work well on Visual Grid")]
        public void TestAcmeTable()
        {
            GetDriver().Url = "file:///C:/temp/fluentexample/Account%20-%20ACME.html";
            GetEyes().Check("main window with table",
                    Target.Window()
                            .Fully()
                            .Ignore(By.ClassName("toolbar"))
                            .Layout(By.Id("orders-list-desktop"), By.ClassName("snapshot-topic"), By.Id("results-count"))
                            .Strict()
            );
        }

        [Test]
        public void TestAcmeLogin()
        {
            GetDriver().Url = "https://afternoon-savannah-68940.herokuapp.com/#";
            IWebElement username = GetDriver().FindElement(By.Id("username"));
            username.SendKeys("adamC");
            IWebElement password = GetDriver().FindElement(By.Id("password"));
            password.SendKeys("MySecret123?");
            GetEyes().Check(
                    Target.Region(username),
                    Target.Region(password)
            );
        }
    }
}