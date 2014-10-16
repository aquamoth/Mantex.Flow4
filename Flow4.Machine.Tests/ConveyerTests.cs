using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow4.Framework;
using System.Threading.Tasks;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class ConveyerTests
    {
        FeedFactory feedFactory;
        Conveyer conveyer;

        public ConveyerTests()
        {
            feedFactory = new FeedFactory();
            conveyer = new Conveyer(feedFactory);
        }

        [TestMethod]
        public void Conveyer_inits_with_unknown_position()
        {
            Assert.IsNull(conveyer.Position);
        }

        [TestMethod]
        public void Conveyer_resets_position()
        {
            conveyer.ResetPosition();
            Assert.AreEqual(0, conveyer.Position.Value);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void Conveyer_waits_for_position_reset_when_started()
        {
            conveyer.Start().Wait();
            Assert.IsNull(conveyer.Position);
            Task.Delay(500).Wait();
            Assert.IsNull(conveyer.Position);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void Conveyer_tracks_position_when_started()
        {
            conveyer.Start().Wait();
            conveyer.ResetPosition();
            Assert.AreEqual(0, conveyer.Position.Value);
            Task.Delay(500).Wait();
            Assert.AreNotEqual(0, conveyer.Position.Value);
        }



    }
}
