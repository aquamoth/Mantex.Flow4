using Flow4.Entities;
using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Flow4.Machine
{
    public class Scanner : BaseMachineController, IScanner
    {
        public Scanner() 
            : base(100)
        {
            ScanlinePool.Instance.NumberOfPixels = 1024;
        }

        public IDetector Detector { get; set; }
        public IXray Xray { get; set; }

        public override async Task<bool> Start()
        {
            var success = await Xray.Start();
            if (!success)
                return false;
            success = await Detector.Start();
            if (!success)
            {
                await Xray.Stop();
                return false;
            }
            success = await base.Start();
            if (!success)
            {
                await Detector.Stop();
                await Xray.Stop();
                return false;
            }
            return true;
        }

        public override async Task<bool> Stop()
        {
            var success = await base.Stop();
            success &= await Detector.Stop();
            success &= await Xray.Stop();
            return success;
        }

        //protected override void OnHeartbeat(object state)
        //{
        //    processNewScanlines();
        //    base.OnHeartbeat(state);
        //}

        //private void processNewScanlines()
        //{
        //    var scanLines = readScanlineFromHardware();
        //    //Task.Run(() =>
        //    //{
        //        var committedFrames = new Queue<IFrame>();
        //        lock (_lockObject)
        //        {
        //            foreach (var scanLine in scanLines)
        //            {
        //                frameBuilder.Lines.Add(scanLine);
        //                if (frameBuilder.Lines.Count == 100)
        //                {
        //                    committedFrames.Enqueue(frameBuilder.Commit());
        //                    frameBuilder = new FrameBuilder();
        //                }
        //            }
        //        }
        //        foreach (var frame in committedFrames)
        //        {
        //            OnFrameCreated(new FrameCreatedEventArgs { Frame = frame });
        //        }
        //    //});
        //}

        //Random _rnd = new Random();
        //private IEnumerable<IScanline> readScanlineFromHardware()
        //{
        //    var numberOfNewScanlinesInHardwareBuffer = _rnd.Next(9, 12);
        //    var builders = ScanlinePool.Instance.Take(numberOfNewScanlinesInHardwareBuffer);
        //    foreach(var scanLine in builders)
        //    {
        //        _rnd.NextBytes(scanLine.Pixels);
        //        yield return scanLine.Commit();
        //    }
        //}

        //public event EventHandler<FrameCreatedEventArgs> FrameCreated;
        //protected void OnFrameCreated(FrameCreatedEventArgs e)
        //{
        //    Trace.TraceInformation("Created frame {0}", e.Frame.Id);
        //    if (FrameCreated != null)
        //        FrameCreated(this, e);
        //}
    }
}
