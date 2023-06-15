using Applitools.Selenium;
using Applitools.VisualGrid;
using NUnit.Framework;

namespace Eyes.Selenium.UnitTests
{
    public class DontCloseBatchesTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void VisualGridRunner_SetRunnerDontCloseBatchesAfter_DontCloseBatchesPopulatedForEyes(bool? dontCloseBatches)
        {
            var runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));
            var eyes = new Applitools.Selenium.Eyes(runner);
            runner.DontCloseBatches = dontCloseBatches;

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void ClassicRunner_SetRunnerDontCloseBatchesAfter_DontCloseBatchesPopulatedForEyes(bool? dontCloseBatches)
        {
            var runner = new ClassicRunner();
            var eyes = new Applitools.Selenium.Eyes(runner);
            runner.DontCloseBatches = dontCloseBatches;

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void VisualGridRunner_SetRunnerDontCloseBatchesBefore_DontCloseBatchesPopulatedForEyes(bool? dontCloseBatches)
        {
            var runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));
            runner.DontCloseBatches = dontCloseBatches;
            var eyes = new Applitools.Selenium.Eyes(runner);

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void ClassicRunner_SetRunnerDontCloseBatchesBefore_DontCloseBatchesPopulatedForEyes(bool? dontCloseBatches)
        {
            var runner = new ClassicRunner();
            runner.DontCloseBatches = dontCloseBatches;
            var eyes = new Applitools.Selenium.Eyes(runner);

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void VisualGridRunner_OverrideDontCloseBatches_DontCloseBatchesPopulatedForEyes(bool? dontCloseBatches)
        {
            var runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));
            var eyes = new Applitools.Selenium.Eyes(runner);
            runner.DontCloseBatches = !dontCloseBatches;
            eyes.DontCloseBatches = dontCloseBatches;

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void ClassicRunner_OverrideDontCloseBatches_DontCloseBatchesPopulatedForEyes(bool? dontCloseBatches)
        {
            var runner = new ClassicRunner();
            var eyes = new Applitools.Selenium.Eyes(runner);
            runner.DontCloseBatches = !dontCloseBatches;
            eyes.DontCloseBatches = dontCloseBatches;

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        public void DontCloseBatches_VisualGridRunner_NullIsDefault()
        {
            var runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5));
            var eyes = new Applitools.Selenium.Eyes(runner);

            Assert.IsNull(eyes.DontCloseBatches);
        }

        [Test]
        public void DontCloseBatches_ClassicRunner_NullIsDefault()
        {
            var runner = new ClassicRunner();
            var eyes = new Applitools.Selenium.Eyes(runner);

            Assert.IsNull(eyes.DontCloseBatches);
        }
    }
}