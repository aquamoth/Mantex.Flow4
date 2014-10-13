using Flow4.Entities;
using Flow4.Framework;
using Flow4.Sample.Controllers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Detector : BaseMachineController, IDetector
    {
        const int SCANLINES_PER_FRAME = 1000;

        object _lockObject = new object();

        IFeed<IFrame> frameFeedHighEnergy;
        IFeed<IFrame> frameFeedLowEnergy;

        FrameBuilder frameBuilderHighEnergy = null;
        FrameBuilder frameBuilderLowEnergy = null;

        public Detector(IFeedFactory feedFactory)
            : base(500)
        {
            frameFeedHighEnergy = feedFactory.GetFeedOf<IFrame>("RawHighEnergyFrameFeed");
            frameFeedLowEnergy = feedFactory.GetFeedOf<IFrame>("RawLowEnergyFrameFeed");
        }

        protected override async Task<bool> OnStart()
        {
            frameBuilderHighEnergy = new FrameBuilder();
            frameBuilderLowEnergy = new FrameBuilder();

            var success = await base.OnStart();
            return success;
        }

        protected override async Task<bool> OnStop()
        {
            frameBuilderHighEnergy.Dispose();
            frameBuilderHighEnergy = null;

            frameBuilderLowEnergy.Dispose();
            frameBuilderLowEnergy = null;

            var success = await base.Stop();
            return success;
        }

        protected override void OnHeartbeat()
        {
            base.OnHeartbeat();
            processNewScanlines();
        }

        private void processNewScanlines()
        {
            var scanLinesHighEnergy = readScanlineFromHardware();
            var scanLinesLowEnergy = readScanlineFromHardware();

            //Task.Run(() =>
            //{
            var committedFramesHighEnergy = new Queue<IFrame>();
            var committedFramesLowEnergy = new Queue<IFrame>();
            lock (_lockObject)
            {
                appendScanlinesToFrameBuilder(scanLinesHighEnergy, committedFramesHighEnergy, ref frameBuilderHighEnergy);
                appendScanlinesToFrameBuilder(scanLinesLowEnergy, committedFramesLowEnergy, ref frameBuilderLowEnergy);

                foreach (var frame in committedFramesHighEnergy)
                {
                    frameFeedHighEnergy.Enqueue(frame);
                    frame.Dispose();
                    //Trace.TraceInformation("Wrote frame {0}", frame.Id);
                    //OnFrameCreated(new FrameCreatedEventArgs { Frame = frame });
                }
                foreach (var frame in committedFramesLowEnergy)
                {
                    frameFeedLowEnergy.Enqueue(frame);
                    frame.Dispose();
                    //Trace.TraceInformation("Wrote frame {0}", frame.Id);
                    //OnFrameCreated(new FrameCreatedEventArgs { Frame = frame });
                }
            }
            //});
        }

        private void appendScanlinesToFrameBuilder(IEnumerable<IScanline> scanLines, Queue<IFrame> queueOfCommittedFrames, ref FrameBuilder frameBuilder)
        {
            foreach (var scanLine in scanLines)
            {
                frameBuilder.Lines.Add(scanLine);
                if (frameBuilder.Lines.Count == SCANLINES_PER_FRAME)
                {
                    queueOfCommittedFrames.Enqueue(frameBuilder.Commit());
                    frameBuilder = new FrameBuilder();
                }
            }
        }

        //int DEBUG_READ_COUNTER = 8;
        Random _rnd = new Random();
        private IEnumerable<IScanline> readScanlineFromHardware()
        {

            //if (DEBUG_READ_COUNTER <= 0)
            //    yield break;
            //DEBUG_READ_COUNTER--;

            var numberOfNewScanlinesInHardwareBuffer = _rnd.Next(480, 520);
            var builders = ScanlinePool.Instance.Take(numberOfNewScanlinesInHardwareBuffer);
            foreach (var scanLine in builders)
            {
                _rnd.NextBytes(scanLine.Pixels);
                yield return scanLine.Commit();
            }
        }

    }
}
