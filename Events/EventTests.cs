using ThreadBound.Events.Domain;

namespace ThreadBound.Events;

[TestClass]
public class EventTests
{
    [TestMethod]
    public void SingleSubscriberReceivesEvent()
    {
        var publisher = new EventPublisher();
        var subscriber = new EventSubscriber();

        publisher.EventOccurred += subscriber.OnEventOccurred!;
        publisher.Trigger("Hello, World!", 42);

        Assert.AreEqual(1, subscriber.Received.Count);
        Assert.AreEqual("Hello, World!", subscriber.Received[0].Message);
        Assert.AreEqual(42, subscriber.Received[0].Value);
    }

    [TestMethod]
    public void MultipleSubscribersReceiveEvent()
    {
        var publisher = new EventPublisher();
        var s1 = new EventSubscriber();
        var s2 = new EventSubscriber();

        publisher.EventOccurred += s1.OnEventOccurred!;
        publisher.EventOccurred += s2.OnEventOccurred!;
        publisher.Trigger("Event for all", 100);

        Assert.AreEqual(1, s1.Received.Count);
        Assert.AreEqual(1, s2.Received.Count);
        Assert.AreEqual("Event for all", s1.Received[0].Message);
        Assert.AreEqual(100, s1.Received[0].Value);
        Assert.AreEqual("Event for all", s2.Received[0].Message);
        Assert.AreEqual(100, s2.Received[0].Value);
    }

    [TestMethod]
    public void UnsubscribeStopsReceivingEvent()
    {
        var publisher = new EventPublisher();
        var subscriber = new EventSubscriber();

        publisher.EventOccurred += subscriber.OnEventOccurred!;
        publisher.Trigger("First event", 1);
        publisher.EventOccurred -= subscriber.OnEventOccurred!;
        publisher.Trigger("Second event", 2);

        Assert.AreEqual(1, subscriber.Received.Count);
        Assert.AreEqual("First event", subscriber.Received[0].Message);
        Assert.AreEqual(1, subscriber.Received[0].Value);
    }
}
