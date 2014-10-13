using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Flow4.Entities.Base;
using System.Collections;

namespace Flow4.Framework.Tests
{
    [TestClass]
    public class FeedTests
    {
        IFeed<TestValue> feed;

        public FeedTests()
        {
            feed = new Feed<TestValue>("TestValues", 3);
        }

        [TestMethod]
        public void Feeds_initializes_as_not_alive()
        {
            Assert.AreEqual(false, feed.IsAlive);
        }

        [TestMethod]
        public void Feeds_are_alive_when_started()
        {
            feed.Start();
            Assert.AreEqual(true, feed.IsAlive);
        }

        [TestMethod]
        public void Feeds_can_be_subscribed_to()
        {
            var feedOutput = feed.Subscribe();
            Assert.IsNotNull(feedOutput);
        }

        [TestMethod]
        public void Feeds_without_subscribers_dont_overflow()
        {
            feed.Enqueue(new TestValue(1));
            feed.Enqueue(new TestValue(2));
            feed.Enqueue(new TestValue(3));
        }

        [TestMethod]
        [ExpectedException(typeof(StackOverflowException))]
        public void Feeds_with_subscribers_can_overflow()
        {
            var subscriber = feed.Subscribe();
            feed.Enqueue(new TestValue(1));
            (subscriber as IEnumerator).MoveNext();
            feed.Enqueue(new TestValue(2));
            feed.Enqueue(new TestValue(3));
            feed.Enqueue(new TestValue(4));
        }

        [TestMethod]
        public void Feeds_dont_overflow_when_subscriber_reads()
        {
            var subscriber = feed.Subscribe();
            feed.Enqueue(new TestValue(1));
            CollectionAssert.AreEqual(new[] { 1 }, subscriber.Select(x => x.Value).ToArray());
            feed.Enqueue(new TestValue(2));
            CollectionAssert.AreEqual(new[] { 2 }, subscriber.Select(x => x.Value).ToArray());
            feed.Enqueue(new TestValue(3));
            CollectionAssert.AreEqual(new[] { 3 }, subscriber.Select(x => x.Value).ToArray());
            feed.Enqueue(new TestValue(4));
            CollectionAssert.AreEqual(new[] { 4 }, subscriber.Select(x => x.Value).ToArray());
        }

        [TestMethod]
        public void Feeds_multiple_subscribers_read_at_different_pace()
        {
            var subscriber1 = feed.Subscribe();
            var subscriber2 = feed.Subscribe();
            feed.Enqueue(new TestValue(1));
            CollectionAssert.AreEqual(new[] { 1 }, subscriber1.Select(x => x.Value).ToArray());
            feed.Enqueue(new TestValue(2));
            CollectionAssert.AreEqual(new[] { 1, 2 }, subscriber2.Select(x => x.Value).ToArray());
            CollectionAssert.AreEqual(new[] { 2 }, subscriber1.Select(x => x.Value).ToArray());
        }

        [TestMethod]
        public void Feeds_factory_creates_named_feeds_once()
        {
            var factory = new FeedFactory();
            var feed = factory.GetFeedOf<TestValue>("Test");
            Assert.IsNotNull(feed);
            var feed2 = factory.GetFeedOf<TestValue>("Test");
            Assert.AreSame(feed, feed2);
        }

        [TestMethod]
        public void Feeds_factory_creates_different_named_feeds()
        {
            var factory = new FeedFactory();
            var feed = factory.GetFeedOf<TestValue>("Test");
            Assert.IsNotNull(feed);
            var feed2 = factory.GetFeedOf<TestValue>("Test2");
            Assert.AreNotSame(feed, feed2);
        }


    }

    class TestValue : BaseRefCountedEntity
    {
        public int Value { get; set; }
        public TestValue(int value) { Value = value; }
    }
}
