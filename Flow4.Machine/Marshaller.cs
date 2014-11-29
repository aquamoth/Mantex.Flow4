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
    public class Marshaller : BaseMachineController, IMarshaller
    {
        object _lockObject = new object();
        FeedOutputQueue<IFrame> _highEnergyFrameFeedQueue;
        FeedOutputQueue<IFrame> _lowEnergyFrameFeedQueue;

        public int HighFeedRetrieveCounter { get; private set; }
        public int LowFeedRetrieveCounter { get; private set; }

        internal IFrame _lastHighFrame = null;
        internal IFrame _lastLowFrame = null;

        public Marshaller(IFeedFactory feedFactory)
            : base(1000)
        {
            var highEnergyFrameFeed = feedFactory.GetFeedOf<IFrame>("RawHighEnergyFrameFeed");
            _highEnergyFrameFeedQueue = highEnergyFrameFeed.Subscribe();

            var lowEnergyFrameFeed = feedFactory.GetFeedOf<IFrame>("RawLowEnergyFrameFeed");
            _lowEnergyFrameFeedQueue = lowEnergyFrameFeed.Subscribe();
            
            Trace.TraceInformation("Marshaller initialized ({0} ,{1}).", _highEnergyFrameFeedQueue.Count, _lowEnergyFrameFeedQueue.Count);
        }

        protected override void Dispose(bool disposing)
        {
            _highEnergyFrameFeedQueue.Dispose();
            _lowEnergyFrameFeedQueue.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnHeartbeat()
        {
            base.OnHeartbeat();

            bool firstRun = true;//TODO
            const double detectorWidth = 10.0;//mm
            const double detectorDistance = 300.0;//mm
            const double detectorFrequency = 1000.0; //lines/s
            var frameOffset = detectorDistance;

            lock(_lockObject)
            {
                Trace.TraceInformation("Marshaller heartbeat ({0} ,{1}).", _highEnergyFrameFeedQueue.Count, _lowEnergyFrameFeedQueue.Count);

                //We realign the high energy frame.

                //Get the next low energy frame if we need one and one is available.
                if (_lastLowFrame == null)
                {
                    if (_lowEnergyFrameFeedQueue.Count == 0)
                        return;
                    _lastLowFrame = _lowEnergyFrameFeedQueue.First();
                }

                //Get the next part of the high energy frame to realign
                if (_highEnergyFrameFeedQueue.Count > 0)
                {
                    var high = _highEnergyFrameFeedQueue.First();
                    var highLines = high.Lines;

                    var conveyerSpeed = 1500.0;// mm/s
                    var currentDetectorLineWidth = conveyerSpeed / detectorFrequency; // mm/line

                    if(firstRun)
                    {
                        // Just throw await the part of the first frame that doesn't map against the first low energy frame
                        var frameOffsetInPixels = frameOffset / conveyerSpeed * detectorFrequency;
                        highLines = highLines.Skip((int)frameOffsetInPixels);
                        var alignedHighEnergyFrameBuilder = new FrameBuilder();

                        frameOffset-= frameOffsetInPixels/detectorFrequency*conveyerSpeed;
                    }

                    ////Add as many high energy lines as needed to fill a frame
                    //while(true)
                    //{
                    //    alignedHighEnergyFrameBuilder.Lines.Add
                    //}
                    
                    



                    foreach (var frame in _highEnergyFrameFeedQueue)
                    {
                        Trace.TraceInformation("Read high energy frame {0}.", frame.Id);
                        frame.Dispose();
                        HighFeedRetrieveCounter++;
                    }

                    foreach (var frame in _lowEnergyFrameFeedQueue)
                    {
                        Trace.TraceInformation("Read low energy frame {0}.", frame.Id);
                        frame.Dispose();
                        LowFeedRetrieveCounter++;
                    }


                }
            }
        }
    }
}
