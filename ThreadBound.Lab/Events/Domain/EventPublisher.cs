using System;

namespace ThreadBound.Events.Domain;

public class EventPublisher
{
    public event EventHandler<CustomEventArgs>? EventOccurred;

    public void Trigger(string message, int value)
    {
        Raise(new CustomEventArgs(message, value));
    }

    protected virtual void Raise(CustomEventArgs args)
    {
        var invocationList = EventOccurred?.GetInvocationList();
        if (invocationList == null) return;

        var actions = invocationList
            .Cast<EventHandler<CustomEventArgs>>()
            .Select(handler => (Action)(() => handler(this, args)))
            .ToArray();

        Parallel.Invoke(actions);
    }
}

