using Flow4.Entities.Base;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Flow4.Framework
{
    public class FeedFactory : IFeedFactory
    {
        Dictionary<string, IFeed> feeds;
        int capacity;

        public FeedFactory(int capacity = 100)
        {
            this.feeds = new Dictionary<string, IFeed>();
            this.capacity = capacity;
        }

        public void Dispose()
        {
            foreach (var feed in feeds.Values)
            {
                feed.Dispose();
            }
            feeds = null;
        }

        public IFeed<T> GetFeedOf<T>(string name)
            where T: IRefCountedEntity
        {
            if(feeds.ContainsKey(name))
            {
                var feed = feeds[name] as Feed<T>;
                if (feed != null)
                    return feed;
                throw new NotSupportedException(
                    string.Format("Expected feed {0} to be of type {1} but it is actually of type {2}", name, typeof(T), feeds[name].GetType())
                    );
            }
            else
            {
                var feed = new Feed<T>(name, capacity);
                feed.Start();
                feeds.Add(name, feed as IFeed);
                return feed;
            }
        }
    }
}
