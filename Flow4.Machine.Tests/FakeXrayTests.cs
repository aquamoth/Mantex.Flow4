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
            xray = new FakeXRay(100, 100);
        }

        [TestMethod]
        public void FakeXray_Initializes_as_stopped()
        {
            Assert.AreEqual(Machine.State.Stopped, xray.State);
        }

        [TestMethod]
        public void FakeXray_Cant_be_stopped_when_not_running()
        {
            var stateChangedCalled = false;
            xray.StateChanged += (sender, e) =>
            {
                stateChangedCalled = true;
            };
            xray.Stop().Wait();
            Assert.AreNotEqual(true, stateChangedCalled);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void FakeXray_Can_be_started_and_stopped()
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
        public void FakeXray_Cant_be_started_when_running()
        {
            var startTask = xray.Start();
            xray.Start().Wait();
        }
    }
}
