using System;

namespace ThreadBound.Events.Domain;

public class CustomEventArgs : EventArgs
{
    private readonly string _message;
    private readonly int _value;

    public CustomEventArgs(string message, int value)
    {
        _message = message;
        _value = value;
    }

    public string Message => _message;
    public int Value => _value;
}

