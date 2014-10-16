using Flow4.Entities;
using Flow4.Entities.Base;
using Flow4.Framework;
using Flow4.IMachine;
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

        IFeed<IFrame> _frameFeedHighEnergy;
        IFeed<IFrame> _frameFeedLowEnergy;

        FrameBuilder _frameBuilderHighEnergy = null;
        FrameBuilder _frameBuilderLowEnergy = null;

        IPool<ScanlineBuilder> _scanLinePool;

        public Detector(IFeedFactory feedFactory, IScanlinePool scanlinePool)
            : base(500)
        {
            this._frameFeedHighEnergy = feedFactory.GetFeedOf<IFrame>("RawHighEnergyFrameFeed");
            this._frameFeedLowEnergy = feedFactory.GetFeedOf<IFrame>("RawLowEnergyFrameFeed");
            this._scanLinePool = scanlinePool;
        }

        protected override async Task<bool> OnStart()
        {
            _frameBuilderHighEnergy = new FrameBuilder();
            _frameBuilderLowEnergy = new FrameBuilder();

            var success = await base.OnStart();
            return success;
        }

        protected override async Task<bool> OnStop()
        {
            var success = await base.OnStop();

            _frameBuilderHighEnergy.Dispose();
            _frameBuilderHighEnergy = null;

            _frameBuilderLowEnergy.Dispose();
            _frameBuilderLowEnergy = null;

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

            var committedFramesHighEnergy = new Queue<IFrame>();
            var committedFramesLowEnergy = new Queue<IFrame>();
            lock (_lockObject)
            {
                appendScanlinesToFrameBuilder(scanLinesHighEnergy, committedFramesHighEnergy, ref _frameBuilderHighEnergy);
                appendScanlinesToFrameBuilder(scanLinesLowEnergy, committedFramesLowEnergy, ref _frameBuilderLowEnergy);

                foreach (var frame in committedFramesHighEnergy)
                {
                    _frameFeedHighEnergy.Enqueue(frame);
                    frame.Dispose();
                }
                foreach (var frame in committedFramesLowEnergy)
                {
                    _frameFeedLowEnergy.Enqueue(frame);
                    frame.Dispose();
                }
            }
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
            var numberOfNewScanlinesInHardwareBuffer = _rnd.Next(480, 520);
            var builders = _scanLinePool.Take(numberOfNewScanlinesInHardwareBuffer);
            foreach (var scanLine in builders)
            {
                _rnd.NextBytes(scanLine.Pixels);
                yield return scanLine.Commit();
            }
        }

    }
}
