using System;

namespace ThreadBound.Events.Domain;

public class EventSubscriber
{
    private readonly List<CustomEventArgs> _received = new List<CustomEventArgs>();

    public IReadOnlyList<CustomEventArgs> Received => _received;

    public void OnEventOccurred(object sender, CustomEventArgs args)
    {
        _received.Add(args);
    }
}

