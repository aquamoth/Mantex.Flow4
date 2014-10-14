﻿using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow4.Entities;
using Flow4.Framework;
using System.Threading.Tasks;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class ExecutiveTests
    {
        IExecutive executive;
        Marshaller marshaller;

        public ExecutiveTests()
        {
            var feedFactory = new FeedFactory();
            
            var scanner = new Scanner();
            scanner.Detector = new Detector(feedFactory);
            scanner.Xray = new FakeXRay(0, 0);

            marshaller = new Marshaller(feedFactory);
            
            executive = new Executive(scanner, marshaller);
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
        public void Executive_routes_detector_frames_to_marshaller()
        {
            Assert.AreEqual(0, marshaller.HighFeedRetrieveCounter);
            Assert.AreEqual(0, marshaller.LowFeedRetrieveCounter);

            executive.Start().Wait();
            Task.Delay(2000).Wait();

            Assert.AreNotEqual(0, marshaller.HighFeedRetrieveCounter);
            Assert.AreNotEqual(0, marshaller.LowFeedRetrieveCounter);
        }

    }
}
