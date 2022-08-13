using Applitools;
using Applitools.VisualGrid;
using NUnit.Framework;

namespace Eyes.Selenium.UnitTests
{
    public class DontCloseBatchesTests
    {
        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DontCloseBatches_VisualGridRunner_DontCloseBatchesPopulatedForEyes(bool dontCloseBatches)
        {
            var runner = new VisualGridRunner(new RunnerOptions().TestConcurrency(5), startServer:false);
            var eyes = new Applitools.Selenium.Eyes(runner);
            runner.DontCloseBatches = dontCloseBatches;

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }

        [Test]
        [TestCase(true)]
        [TestCase(false)]
        public void DontCloseBatches_ClassicRunner_DontCloseBatchesPopulatedForEyes(bool dontCloseBatches)
        {
            var runner = new ClassicRunner(false);
            var eyes = new Applitools.Selenium.Eyes(runner);
            runner.DontCloseBatches = dontCloseBatches;

            Assert.AreEqual(dontCloseBatches, eyes.DontCloseBatches);
        }
    }
}