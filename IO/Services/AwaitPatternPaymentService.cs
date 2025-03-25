using ThreadBound.IO.Domain;

namespace ThreadBound.IO.Services;

public static class AwaitPatternPaymentService
{
    private static readonly List<(int index, string note)> _notifications = new();
    private static readonly object _syncLock = new();

    public static IReadOnlyList<(int index, string note)> Notifications => _notifications;

    public static async Task ProcessPaymentsAsync()
    {
        _notifications.Clear();
        var items = new List<PaymentItem>
            {
                new(0, 3000, "Payment processed for Order 0"),
                new(1, 1000, "Payment processed for Order 1"),
                new(2, 2000, "Payment processed for Order 2"),
                new(3,  500, "Payment processed for Order 3")
            };

        var tasks = new List<Task>();
        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);
                var note = $"Notification: Payment {item.Index} processed - {item.Data}";
                lock (_syncLock)
                {
                    _notifications.Add((item.Index, note));
                }
            }));
        }

        await Task.WhenAll(tasks);
    }

    public static async Task ProcessPaymentsWithException()
    {
        _notifications.Clear();
        var items = new List<PaymentItem>
            {
                new(0, 3000, "Payment processed for Order 0"),
                new(1, 1000, "Payment processed for Order 1"),
                new(2, 2000, "Payment processed for Order 2"),
                new(3,  500, "Payment processed for Order 3")
            };

        var tasks = new List<Task>();
        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);
                if (item.Index == 1)
                    throw new InvalidOperationException("Simulated exception in inline notification");
                var note = $"Notification: Payment {item.Index} processed - {item.Data}";
                lock (_syncLock)
                {
                    _notifications.Add((item.Index, note));
                }
            }));
        }

        await Task.WhenAll(tasks);
    }
}