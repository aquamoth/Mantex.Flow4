using Flow4.Entities;
using Flow4.Framework;
using Flow4.IMachine;
using Flow4.Machine.Tests.Mocks;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using System.Threading;

namespace Flow4.Machine.Tests
{
    [TestClass]
    public class ScannerTests
    {
        Scanner scanner;

        public ScannerTests()
        {
            scanner = new Scanner
            {
                Xray = new FakeXRay(0, 0),
                Detector = new Detector(new FeedFactory(), new ScanlinePool(1024))
            };
        }

        [TestMethod]
        public void Scanner_inits_as_stopped()
        {
            Assert.AreEqual(State.Stopped, scanner.State);
        }

        [TestMethod]
        public void Scanner_can_be_started_and_stopped()
        {
            scanner.Start().Wait();
            Assert.AreEqual(State.Running, scanner.State);
            scanner.Stop().Wait();
            Assert.AreEqual(State.Stopped, scanner.State);
        }
        
        [TestMethod]
        [TestCategory("Slow")]
        public void Scanner_fails_when_xray_breaks()
        {
            scanner.Xray = new BreakingXRay();
            scanner.Detector = new Detector(new FeedFactory(), new ScanlinePool(1024));
            scanner.Start().Wait();

            waitForScannerToFail();

            Assert.AreEqual(State.Failure, scanner.State);
        }

        [TestMethod]
        [TestCategory("Slow")]
        public void Scanner_stops_xray_if_detector_fails()
        {
            scanner.Xray = new FakeXRay(0, 0);
            scanner.Detector = new FailingDetector();
            scanner.Start().Wait();
            waitForScannerToFail();
            Task.Delay(100).Wait();

            Assert.AreEqual(State.Stopped, scanner.Xray.State);
        }





        private void waitForScannerToFail()
        {
            var cancellationToken = new CancellationTokenSource();
            var delay = Task.Delay(5000, cancellationToken.Token);
            scanner.StateChanged += (sender, e) =>
            {
                if (scanner.State == State.Failure)
                    cancellationToken.Cancel(false);
            };

            try
            {
                delay.Wait();
            }
            catch { }
        }
    }
}
