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
    class Detector : BaseController
    {
        const int SCANLINES_PER_FRAME = 100;

        object _lockObject = new object();

        Feed<IFrame> frameFeedHighEnergy;
        Feed<IFrame> frameFeedLowEnergy;

        FrameBuilder frameBuilderHighEnergy = null;
        FrameBuilder frameBuilderLowEnergy = null;

        public Detector(HashSet<object> resources)
            : base(100)
        {
            var frameFeedResources = resources.OfType<Feed<IFrame>>();
            frameFeedHighEnergy = frameFeedResources
                .Single(feed => feed.Name == "Raw High Energy Frame");
            frameFeedLowEnergy = frameFeedResources
                .Single(feed => feed.Name == "Raw Low Energy Frame");
        }

        public override void Start()
        {
            frameBuilderHighEnergy = new FrameBuilder();
            frameBuilderLowEnergy = new FrameBuilder();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            
            frameBuilderHighEnergy.Dispose();
            frameBuilderHighEnergy = null;

            frameBuilderLowEnergy.Dispose();
            frameBuilderLowEnergy = null;
        }

        protected override void OnHeartbeat(object state)
        {
            processNewScanlines();
            base.OnHeartbeat(state);
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
                    //Trace.TraceInformation("Wrote frame {0}", frame.Id);
                    //OnFrameCreated(new FrameCreatedEventArgs { Frame = frame });
                }
                foreach (var frame in committedFramesLowEnergy)
                {
                    frameFeedLowEnergy.Enqueue(frame);
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

        Random _rnd = new Random();
        private IEnumerable<IScanline> readScanlineFromHardware()
        {
            var numberOfNewScanlinesInHardwareBuffer = _rnd.Next(9, 12);
            var builders = ScanlinePool.Instance.Take(numberOfNewScanlinesInHardwareBuffer);
            foreach (var scanLine in builders)
            {
                _rnd.NextBytes(scanLine.Pixels);
                yield return scanLine.Commit();
            }
        }

        //public event EventHandler<FrameCreatedEventArgs> FrameCreated;
        //protected void OnFrameCreated(FrameCreatedEventArgs e)
        //{
        //    Trace.TraceInformation("Created frame {0}", e.Frame.Id);
        //    if (FrameCreated != null)
        //        FrameCreated(this, e);
        //}

    }
}
