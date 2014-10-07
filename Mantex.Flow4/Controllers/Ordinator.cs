using Flow4.Entities;
using Flow4.Framework;
using Flow4.Machine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Sample.Controllers
{
    public class Ordinator
    {
        Executive executive;
        HashSet<object> resources;

        public Ordinator()
        {
            resources = new HashSet<object>();
            createFrameFeedResource("Raw High Energy Frame");
            createFrameFeedResource("Raw Low Energy Frame");

            executive = new Executive(resources);
        }

        public void Start()
        {
            executive.Start();
        }

        public void Stop()
        {
            executive.Stop();
        }

        private void createFrameFeedResource(string name)
        {
            var feed = new Feed<IFrame>(name);
            feed.Start();
            resources.Add(feed);
        }
    }
}
