using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow4.Framework;
using Flow4.IMachine;
using Flow4.Entities;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class MarshallerTests
    {
        IFeedFactory feedFactory;
        Marshaller marshaller;

        public MarshallerTests()
        {
            feedFactory = new FeedFactory();
            marshaller = new Marshaller(feedFactory);
        }

        [TestMethod]
        public void Marshaller_inits_stopped()
        {
            Assert.AreEqual(State.Stopped, marshaller.State);
        }

        [TestMethod]
        public void Marshaller_can_stop_and_stop()
        {
            marshaller.Start().Wait();
            Assert.AreEqual(State.Running, marshaller.State);
            marshaller.Stop().Wait();
            Assert.AreEqual(State.Stopped, marshaller.State);
        }

        [TestMethod]
        public void Marshaller_receives_feeds()
        {
            //marshaller.Start().Wait();
            //var highFeed = feedFactory.GetFeedOf<IFrame>("RawHighEnergyFrameFeed");
            //var lowFeed = feedFactory.GetFeedOf<IFrame>("RawLowEnergyFrameFeed");
            //Assert.AreEqual(11, marshaller._lastHighFrame);
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Marshaller_reads_conveyer_speed()
        {
            marshaller.Start().Wait();
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Marshaller_syncs_over_constant_speed()
        {
            marshaller.Start().Wait();
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Marshaller_supports_very_slow_speeds()
        {
            marshaller.Start().Wait();
            Assert.Inconclusive();
        }

        [TestMethod]
        public void Marshaller_handles_speed_changes()
        {
            marshaller.Start().Wait();
            Assert.Inconclusive();
        }
    }
}
