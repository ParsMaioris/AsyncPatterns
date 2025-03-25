using ThreadBound.IO.Domain;
using ThreadBound.IO.Enumerables;

namespace ThreadBound.IO.Services;

public static class CustomIteratorPaymentService
{
    public static IAsyncEnumerable<(int index, string data)> GetPayments()
    {
        return new PaymentAsyncEnumerable(BuildPaymentTasks(false));
    }

    public static IAsyncEnumerable<(int index, string data)> GetPaymentsWithException()
    {
        return new PaymentAsyncEnumerable(BuildPaymentTasks(true));
    }

    static List<Task<(int, string)>> BuildPaymentTasks(bool throwOnIndex2)
    {
        var items = new List<PaymentItem>
            {
                new(0, 3000, "Payment processed for Order 0"),
                new(1, 1000, "Payment processed for Order 1"),
                new(2, 2000, "Payment processed for Order 2"),
                new(3,  500, "Payment processed for Order 3")
            };

        var tasks = new List<Task<(int, string)>>();

        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.Delay);

                if (throwOnIndex2 && item.Index == 2)
                    throw new InvalidOperationException("Simulated exception in async iterator");

                return (item.Index, item.Data);
            }));
        }

        return tasks;
    }
}
