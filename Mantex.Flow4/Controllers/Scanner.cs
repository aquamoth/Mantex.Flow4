using Flow4.Entities;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Scanner : BaseController, Flow4.Sample.Controllers.IScanner
    {
        object _lockObject = new object();
        FrameBuilder frameBuilder = null;

        public Scanner() 
            : base(100)
        {
            ScanlinePool.Instance.NumberOfPixels = 1024;
        }

        public override void Start()
        {
            frameBuilder = new FrameBuilder();
            base.Start();
        }

        public override void Stop()
        {
            //lock(_lockObject)
            //{
                base.Stop();
                frameBuilder.Dispose();
                frameBuilder = null;
            //}
        }

        protected override void OnHeartbeat(object state)
        {
            processNewScanlines();
            base.OnHeartbeat(state);
        }

        private void processNewScanlines()
        {
            var scanLines = readScanlineFromHardware();
            //Task.Run(() =>
            //{
                var committedFrames = new Queue<IFrame>();
                lock (_lockObject)
                {
                    foreach (var scanLine in scanLines)
                    {
                        frameBuilder.Lines.Add(scanLine);
                        if (frameBuilder.Lines.Count == 100)
                        {
                            committedFrames.Enqueue(frameBuilder.Commit());
                            frameBuilder = new FrameBuilder();
                        }
                    }
                }
                foreach (var frame in committedFrames)
                {
                    OnFrameCreated(new FrameCreatedEventArgs { Frame = frame });
                }
            //});

            
            
            
            //var list = scanLines.ToArray();
            //scanLines = null;
            //Trace.TraceWarning("Disposing {0} scan lines back to pool.", list.Count());
            //foreach (var x in list)
            //    x.Dispose();
            //Trace.TraceInformation("AFTER Disposing: {0} allocated and {1} free objects.", ScanlinePool.Instance.PerformanceIndex.Item2, ScanlinePool.Instance.PerformanceIndex.Item1);
            //list = null;
        }

        Random _rnd = new Random();
        private IEnumerable<IScanline> readScanlineFromHardware()
        {
            var numberOfNewScanlinesInHardwareBuffer = _rnd.Next(9, 12);
            var builders = ScanlinePool.Instance.Take(numberOfNewScanlinesInHardwareBuffer);
            foreach(var scanLine in builders)
            {
                _rnd.NextBytes(scanLine.Pixels);
                yield return scanLine.Commit();
            }
        }

        public event EventHandler<FrameCreatedEventArgs> FrameCreated;
        protected void OnFrameCreated(FrameCreatedEventArgs e)
        {
            Trace.TraceInformation("Created frame {0}", e.Frame.Id);
            if (FrameCreated != null)
                FrameCreated(this, e);
        }
    }
}
