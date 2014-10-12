using Flow4.Entities;
using Flow4.Framework;
using Flow4.Machine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Ordinator : IOrdinator, IDisposable
    {
        IExecutive executive;
        //HashSet<object> resources;

        public Ordinator(IExecutive executive)
        {
            //resources = new HashSet<object>();
            //createFrameFeedResource("Raw High Energy Frame");
            //createFrameFeedResource("Raw Low Energy Frame");
            //executive = new Executive(resources);
            this.executive = executive;
        }

        public void Start()
        {
            executive.Start();
        }

        public void Stop()
        {
            executive.Stop();
        }

        void IDisposable.Dispose()
        {
            executive.Dispose();

            //foreach (var feed in resources.OfType<Feed<IFrame>>())
            //{
            //    Trace.TraceInformation("Disposing feed '{0}'.", feed.Name);
            //    feed.Stop();
            //    feed.Dispose();
            //}
        }

        //private void createFrameFeedResource(string name)
        //{
        //    var feed = new Feed<IFrame>(name);
        //    feed.Start();
        //    resources.Add(feed);
        //}
    }
}
