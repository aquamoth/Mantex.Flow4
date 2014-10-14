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

        protected override async Task<bool> OnStart()
        {
            if (Xray == null)
                throw new ApplicationException("Xray not defined.");
            if (Detector == null)
                throw new ApplicationException("Detector not defined.");

            this.MonitoredControllers.Add(Xray);
            this.MonitoredControllers.Add(Detector);

            return await base.OnStart();
        }

        protected override async Task<bool> OnStop()
        {
            var success = await base.OnStop();

            this.MonitoredControllers.Remove(Xray);
            this.MonitoredControllers.Remove(Detector);
            
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
