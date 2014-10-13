using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class FakeXrayTests
    {
        IXray xray;

        public FakeXrayTests()
        {
            xray = new FakeXRay();
        }

        [TestMethod]
        public void Initializes_as_stopped()
        {
            Assert.AreEqual(Machine.State.Stopped, xray.State);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void Cant_be_stopped_when_not_running()
        {
            xray.Stop().Wait();
        }

        [TestMethod]
        public void Can_be_started_and_stopped()
        {
            var task = xray.Start();
            Assert.AreEqual(State.Starting, xray.State);
            task.Wait();
            Assert.AreEqual(State.Running, xray.State);
            task = xray.Stop();
            Assert.AreEqual(State.Stopping, xray.State);
            task.Wait();
            Assert.AreEqual(State.Stopped, xray.State);
        }

        [TestMethod]
        [ExpectedException(typeof(AggregateException))]
        public void Cant_be_started_when_running()
        {
            var startTask = xray.Start();
            xray.Start().Wait();
        }
    }
}
