using Flow4.Entities;
using Flow4.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Marshaller : BaseController
    {
        object _lockObject = new object();
        IEnumerable<IFrame> _highEnergyFrameFeed;
        IEnumerable<IFrame> _lowEnergyFrameFeed;

        public Marshaller(HashSet<object> resources)
            : base(5000)
        {
            var frameFeedResources = resources.OfType<Feed<IFrame>>();
            
            _highEnergyFrameFeed = frameFeedResources
                .Single(feed => feed.Name == "Raw High Energy Frame")
                .Subscribe();

            _lowEnergyFrameFeed = frameFeedResources
                .Single(feed => feed.Name == "Raw Low Energy Frame")
                .Subscribe();
        }

        //public void Send(IFrame frame)
        //{
        //    if (!IsAlive)
        //        throw new NotSupportedException("The marshaller is not alive.");

        //    //TODO: Put the object in a queue here to process on the next heartbeat
        //    Trace.TraceInformation("Killing off frame {0}", frame.Id);
        //    frame.Dispose();
        //}

        protected override void OnHeartbeat(object state)
        {
            base.OnHeartbeat(state);

            lock(_lockObject)
            {
                //Trace.TraceInformation("Marshaller heartbeat.");
                foreach (var frame in _highEnergyFrameFeed)
                {
                    Trace.TraceInformation("Read frame {0}.", frame.Id);
                    frame.Dispose();
                }

                foreach (var frame in _lowEnergyFrameFeed)
                {
                    //Trace.TraceInformation("Read frame {0}.", frame.Id);
                    frame.Dispose();
                }
            }
        }

    }
}
