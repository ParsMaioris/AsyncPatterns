using System;

namespace ThreadBound.IO.Domain;

public class PaymentItem
{
    public int Index { get; }
    public int Delay { get; }
    public string Data { get; }

    public PaymentItem(int index, int delay, string data)
    {
        Index = index;
        Delay = delay;
        Data = data;
    }
}
