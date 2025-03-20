namespace EventsDemo;

/// <summary>
/// Custom event arguments used by the publisher.
/// </summary>
public class CustomEventArgs : EventArgs
{
    public string Message { get; }
    public int Value { get; }

    public CustomEventArgs(string message, int value)
    {
        Message = message;
        Value = value;
    }
}

/// <summary>
/// Publisher class that raises an event when something noteworthy occurs.
/// </summary>
public class EventPublisher
{
    /// <summary>
    /// Declare an event using the EventHandler delegate.
    /// </summary>
    public event EventHandler<CustomEventArgs> EventOccurred;

    /// <summary>
    /// Triggers the event by invoking all subscribed handlers.
    /// </summary>
    public void TriggerEvent(string message, int value)
    {
        OnEventOccurred(new CustomEventArgs(message, value));
    }

    /// <summary>
    /// Protected virtual method to raise the event.
    /// </summary>
    protected virtual void OnEventOccurred(CustomEventArgs e)
    {
        // Safe invocation of the event.
        EventOccurred?.Invoke(this, e);
    }
}

/// <summary>
/// Subscriber class that listens for events from the publisher.
/// </summary>
public class EventSubscriber
{
    /// <summary>
    /// A list to store received event data.
    /// </summary>
    public List<CustomEventArgs> ReceivedEvents { get; } = new List<CustomEventArgs>();

    /// <summary>
    /// Handles the event by adding the event data to the list.
    /// </summary>
    public void HandleEvent(object sender, CustomEventArgs e)
    {
        ReceivedEvents.Add(e);
    }
}

[TestClass]
public class EventTests
{
    /// <summary>
    /// Verifies that a single subscriber receives the event.
    /// </summary>
    [TestMethod]
    public void TestSingleSubscriberReceivesEvent()
    {
        // Arrange
        var publisher = new EventPublisher();
        var subscriber = new EventSubscriber();
        publisher.EventOccurred += subscriber.HandleEvent;

        // Act
        publisher.TriggerEvent("Hello, World!", 42);

        // Assert
        Assert.AreEqual(1, subscriber.ReceivedEvents.Count, "Subscriber did not receive the event.");
        Assert.AreEqual("Hello, World!", subscriber.ReceivedEvents[0].Message, "Event message does not match.");
        Assert.AreEqual(42, subscriber.ReceivedEvents[0].Value, "Event value does not match.");
    }

    /// <summary>
    /// Verifies that multiple subscribers receive the event.
    /// </summary>
    [TestMethod]
    public void TestMultipleSubscribersReceiveEvent()
    {
        // Arrange
        var publisher = new EventPublisher();
        var subscriber1 = new EventSubscriber();
        var subscriber2 = new EventSubscriber();

        publisher.EventOccurred += subscriber1.HandleEvent;
        publisher.EventOccurred += subscriber2.HandleEvent;

        // Act
        publisher.TriggerEvent("Event for all", 100);

        // Assert
        Assert.AreEqual(1, subscriber1.ReceivedEvents.Count, "Subscriber1 did not receive the event.");
        Assert.AreEqual(1, subscriber2.ReceivedEvents.Count, "Subscriber2 did not receive the event.");
        Assert.AreEqual("Event for all", subscriber1.ReceivedEvents[0].Message, "Subscriber1: Event message does not match.");
        Assert.AreEqual(100, subscriber1.ReceivedEvents[0].Value, "Subscriber1: Event value does not match.");
        Assert.AreEqual("Event for all", subscriber2.ReceivedEvents[0].Message, "Subscriber2: Event message does not match.");
        Assert.AreEqual(100, subscriber2.ReceivedEvents[0].Value, "Subscriber2: Event value does not match.");
    }

    /// <summary>
    /// Verifies that unsubscribing stops a subscriber from receiving further events.
    /// </summary>
    [TestMethod]
    public void TestUnsubscribeStopsReceivingEvent()
    {
        // Arrange
        var publisher = new EventPublisher();
        var subscriber = new EventSubscriber();

        publisher.EventOccurred += subscriber.HandleEvent;

        // Act: Trigger first event.
        publisher.TriggerEvent("First event", 1);
        // Unsubscribe.
        publisher.EventOccurred -= subscriber.HandleEvent;
        // Trigger second event.
        publisher.TriggerEvent("Second event", 2);

        // Assert: Subscriber should have received only the first event.
        Assert.AreEqual(1, subscriber.ReceivedEvents.Count, "Subscriber should only receive one event after unsubscription.");
        Assert.AreEqual("First event", subscriber.ReceivedEvents[0].Message, "The event message does not match.");
        Assert.AreEqual(1, subscriber.ReceivedEvents[0].Value, "The event value does not match.");
    }
}