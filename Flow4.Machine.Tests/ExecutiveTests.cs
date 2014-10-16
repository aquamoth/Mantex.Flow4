using Flow4.Entities;
using Flow4.Framework;
using Flow4.IMachine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class ExecutiveTests
    {
        IExecutive executive;
        Marshaller marshaller;
        IConveyer conveyer;

        public ExecutiveTests()
        {
            var feedFactory = new FeedFactory();
            var scanlinePool = new ScanlinePool(1024);

            var scanner = new Scanner
            {
                Detector = new Detector(feedFactory, scanlinePool),
                Xray = new FakeXRay(0, 0)
            };
            marshaller = new Marshaller(feedFactory);
            conveyer = new Conveyer(feedFactory);
            
            executive = new Executive(scanner, marshaller, conveyer);
        }

        [TestMethod]
        public void Executive_inits_as_stopped()
        {
            Assert.AreEqual(State.Stopped, executive.State);
        }

        [TestMethod]
        public void Executive_can_be_started_and_stopped()
        {
            executive.Start().Wait();
            Assert.AreEqual(State.Running, executive.State);
            executive.Stop().Wait();
            Assert.AreEqual(State.Stopped, executive.State);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void Executive_routes_detector_frames_to_marshaller()
        {
            Assert.AreEqual(0, marshaller.HighFeedRetrieveCounter);
            Assert.AreEqual(0, marshaller.LowFeedRetrieveCounter);

            executive.Start().Wait();
            Task.Delay(3000).Wait();

            Assert.AreNotEqual(0, marshaller.HighFeedRetrieveCounter);
            Assert.AreNotEqual(0, marshaller.LowFeedRetrieveCounter);
        }

    }
}
