using Flow4.Entities;
using Flow4.Framework;
using Flow4.IMachine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Text;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class FakeDetectorTests
    {
        IDetector detector;
        FeedFactory factory; 

        public FakeDetectorTests()
        {
            factory = new FeedFactory();
            detector = new Detector(factory, new ScanlinePool(1024));
        }

        [TestMethod]
        public void Detector_inits_as_stopped()
        {
            Assert.AreEqual(State.Stopped, detector.State);
        }

        [TestMethod]
        public void Detector_can_be_started_and_stopped()
        {
            detector.Start().Wait();
            Assert.AreEqual(State.Running, detector.State);
            detector.Stop().Wait();
            Assert.AreEqual(State.Stopped, detector.State);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void Detector_writes_frames_to_both_feeds()
        {
            var highFeed = factory.GetFeedOf<IFrame>("RawHighEnergyFrameFeed").Subscribe();
            var lowFeed = factory.GetFeedOf<IFrame>("RawLowEnergyFrameFeed").Subscribe();

            Assert.IsTrue(highFeed.IsEndOfFeed);
            Assert.IsTrue(lowFeed.IsEndOfFeed);

            detector.Start().Wait();
            Task.Delay(2000).Wait();

            Assert.IsFalse(highFeed.IsEndOfFeed);
            Assert.IsFalse(lowFeed.IsEndOfFeed);
        }

    }
}
