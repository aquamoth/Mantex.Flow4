using Flow4.Entities;
using Flow4.Entities.Base;
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

    public class Marshaller : BaseController, IMarshaller
    {
        object _lockObject = new object();
        FeedOutputQueue<IFrame> _highEnergyFrameFeedQueue;
        FeedOutputQueue<IFrame> _lowEnergyFrameFeedQueue;

        public Marshaller(IFeedFactory feedFactory/*HashSet<object> resources*/)
            : base(1000)
        {
            //var feedFactory = kernel.Resolve<IFeedFactory>();
            var highEnergyFrameFeed = feedFactory.GetFeedOf<IFrame>("RawHighEnergyFrameFeed");
            _highEnergyFrameFeedQueue = highEnergyFrameFeed.Subscribe();

            var lowEnergyFrameFeed = feedFactory.GetFeedOf<IFrame>("RawLowEnergyFrameFeed");
            _lowEnergyFrameFeedQueue = lowEnergyFrameFeed.Subscribe();
            
            //var frameFeedResources = resources.OfType<Feed<IFrame>>();
            
            //_highEnergyFrameFeedQueue = frameFeedResources
            //    .Single(feed => feed.Name == "Raw High Energy Frame")
            //    .Subscribe();

            //_lowEnergyFrameFeedQueue = frameFeedResources
            //    .Single(feed => feed.Name == "Raw Low Energy Frame")
            //    .Subscribe();

            Trace.TraceInformation("Marshaller initialized ({0} ,{1}).", _highEnergyFrameFeedQueue.Count, _lowEnergyFrameFeedQueue.Count);
        }

        protected override void Dispose(bool disposing)
        {
            _highEnergyFrameFeedQueue.Dispose();
            _lowEnergyFrameFeedQueue.Dispose();
            base.Dispose(disposing);
        }

        protected override void OnHeartbeat(object state)
        {
            base.OnHeartbeat(state);

            lock(_lockObject)
            {
                Trace.TraceInformation("Marshaller heartbeat ({0} ,{1}).", _highEnergyFrameFeedQueue.Count, _lowEnergyFrameFeedQueue.Count);
                foreach (var frame in _highEnergyFrameFeedQueue)
                {
                    Trace.TraceInformation("Read high energy frame {0}.", frame.Id);
                    frame.Dispose();
                }

                foreach (var frame in _lowEnergyFrameFeedQueue)
                {
                    Trace.TraceInformation("Read low energy frame {0}.", frame.Id);
                    frame.Dispose();
                }
            }
        }
    }
}
