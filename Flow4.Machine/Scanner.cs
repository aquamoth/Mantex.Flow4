using Flow4.Entities;
using Flow4.Framework;
using System;
using System.Collections.Generic;

namespace Flow4.Machine
{
    public class Scanner : BaseController, IScanner
    {
        //object _lockObject = new object();
        //FrameBuilder frameBuilder = null;

        Xray xray;
        //Detector detector;

        //readonly HashSet<object> _resources;

        public Scanner(/*HashSet<object> resources*/) 
            : base(100)
        {
            //this._resources = resources;
            ScanlinePool.Instance.NumberOfPixels = 1024;
        }

        public IDetector detector { get; set; }

        public override void Start()
        {
            xray = new Xray();
            //detector = new Detector(_resources);

            xray.Start();
            detector.Start();

            //frameBuilder = new FrameBuilder();
            base.Start();
        }

        public override void Stop()
        {
            base.Stop();
            detector.Stop();
            xray.Stop();
        }

        //public void Run(IWorkOrder order)
        //{

        //}

        //public override void Stop()
        //{
        //    base.Stop();
        //    frameBuilder.Dispose();
        //    frameBuilder = null;
        //}

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
