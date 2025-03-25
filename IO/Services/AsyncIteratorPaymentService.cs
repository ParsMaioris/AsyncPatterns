using System;
using ThreadBound.IO.Domain;

namespace ThreadBound.IO.Services;

public static class AsyncIteratorPaymentService
{
    public static async IAsyncEnumerable<(int index, string data)> GetPayments()
    {
        var items = new List<PaymentItem>
        {
            new(0, 3000, "Payment processed for Order 0"),
            new(1, 1000, "Payment processed for Order 1"),
            new(2, 2000, "Payment processed for Order 2"),
            new(3,  500, "Payment processed for Order 3")
        };

        var tasks = new List<Task<(int index, string data)>>();
        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);
                return (item.Index, item.Data);
            }));
        }

        while (tasks.Count > 0)
        {
            var finished = await Task.WhenAny(tasks);
            tasks.Remove(finished);
            yield return await finished;
        }
    }

    public static async IAsyncEnumerable<(int index, string data)> GetPaymentsWithException()
    {
        var items = new List<PaymentItem>
        {
            new(0, 3000, "Payment processed for Order 0"),
            new(1, 1000, "Payment processed for Order 1"),
            new(2, 2000, "Payment processed for Order 2"),
            new(3,  500, "Payment processed for Order 3")
        };

        var tasks = new List<Task<(int index, string data)>>();
        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);
                if (item.Index == 2)
                    throw new InvalidOperationException("Simulated exception in async iterator");
                return (item.Index, item.Data);
            }));
        }

        while (tasks.Count > 0)
        {
            var finished = await Task.WhenAny(tasks);
            tasks.Remove(finished);
            yield return await finished;
        }
    }
}
