using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow4.Framework;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class FakeDetectorTests
    {
        IDetector detector;

        public FakeDetectorTests()
        {
            var factory = new FeedFactory();
            detector = new Detector(factory);
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
    }
}
