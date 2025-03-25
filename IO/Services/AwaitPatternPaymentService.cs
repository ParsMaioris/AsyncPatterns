using ThreadBound.IO.Domain;

namespace ThreadBound.IO.Services;

public static class AwaitPatternPaymentService
{
    public static async Task<List<(int index, string data)>> ProcessPaymentsAsync()
    {
        var items = new List<PaymentItem>
        {
            new(0, 3000, "Payment processed for Order 0"),
            new(1, 1000, "Payment processed for Order 1"),
            new(2, 2000, "Payment processed for Order 2"),
            new(3,  500, "Payment processed for Order 3")
        };

        var notifications = new List<(int index, string note)>();
        var tasks = new List<Task<(int index, string data)>>();
        var lockObj = new object();

        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);
                var note = $"Notification: Payment {item.Index} processed - {item.Data}";
                lock (lockObj)
                {
                    notifications.Add((item.Index, note));
                }
                return (item.Index, item.Data);
            }));
        }

        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }

    public static async Task<List<(int index, string data)>> ProcessPaymentsWithException()
    {
        var items = new List<PaymentItem>
        {
            new(0, 3000, "Payment processed for Order 0"),
            new(1, 1000, "Payment processed for Order 1"),
            new(2, 2000, "Payment processed for Order 2"),
            new(3,  500, "Payment processed for Order 3")
        };

        var notifications = new List<(int index, string note)>();
        var tasks = new List<Task<(int index, string data)>>();
        var lockObj = new object();

        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);
                if (item.Index == 1)
                    throw new InvalidOperationException("Simulated exception in inline notification");
                var note = $"Notification: Payment {item.Index} processed - {item.Data}";
                lock (lockObj)
                {
                    notifications.Add((item.Index, note));
                }
                return (item.Index, item.Data);
            }));
        }

        var results = await Task.WhenAll(tasks);
        return results.ToList();
    }
}
