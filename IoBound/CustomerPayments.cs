namespace IoBoundOperations.Tests;

/// <summary>
/// Simulates processing payment data using an asynchronous iterator.
/// </summary>
public static class CustomerPayments
{
    /// <summary>
    /// Asynchronous iterator that launches tasks in parallel and yields results on completion.
    /// </summary>
    /// <returns>An async stream of (index, data) tuples.</returns>
    public static async IAsyncEnumerable<(int index, string data)> GetDataAsync()
    {
        var items = new List<(int index, int delay, string data)>
            {
                (0, 3000, "Payment processed for Order 0"),
                (1, 1000, "Payment processed for Order 1"),
                (2, 2000, "Payment processed for Order 2"),
                (3, 500,  "Payment processed for Order 3")
            };

        // Launch all tasks concurrently.
        var tasks = new List<Task<(int index, string data)>>();
        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.delay);
                return (item.index, item.data);
            }));
        }

        // Yield each result as soon as its task completes.
        while (tasks.Count > 0)
        {
            var finishedTask = await Task.WhenAny(tasks);
            tasks.Remove(finishedTask);
            yield return await finishedTask;
        }
    }
}

/// <summary>
/// MSTest class demonstrating I-O bound operations using an asynchronous iterator.
/// </summary>
[TestClass]
public class IoBoundOperationsTests
{
    /// <summary>
    /// Uses the asynchronous iterator to process payment data on demand.
    /// Verifies that tasks with lower delays complete before those with longer delays.
    /// </summary>
    [TestMethod]
    public async Task TestGetDataAsync()
    {
        var results = new List<(int index, string data)>();

        // Process each payment result as it becomes available.
        await foreach (var result in CustomerPayments.GetDataAsync())
        {
            results.Add(result);
        }

        // We expect 4 results.
        Assert.AreEqual(4, results.Count, "Expected 4 results from GetDataAsync.");

        // Given the delays (Order 3: 500ms, Order 1: 1000ms, Order 2: 2000ms, Order 0: 3000ms),
        // the expected order of completion is: 3, 1, 2, 0.
        var expectedOrder = new List<int> { 3, 1, 2, 0 };
        CollectionAssert.AreEqual(expectedOrder, results.ConvertAll(r => r.index), "The returned indices do not match the expected order based on delay times.");
    }

    /// <summary>
    /// Demonstrates the await-notification inline pattern.
    /// Each payment is processed in a Task.Run; immediately after awaiting the delay,
    /// a notification is sent (simulated by adding to a shared notifications list).
    /// </summary>
    [TestMethod]
    public async Task TestPaymentNotificationInline()
    {
        var items = new List<(int index, int delay, string data)>
            {
                (0, 3000, "Payment processed for Order 0"),
                (1, 1000, "Payment processed for Order 1"),
                (2, 2000, "Payment processed for Order 2"),
                (3, 500,  "Payment processed for Order 3")
            };

        // List to capture notifications immediately after each payment completes.
        var notifications = new List<(int index, string notification)>();
        object lockObj = new object();

        var tasks = new List<Task<(int index, string data)>>();

        // For each item, start a task that awaits the delay, then immediately sends a notification.
        foreach (var item in items)
        {
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(item.delay);
                // Immediately send notification inline after the await.
                string notification = $"Notification: Payment {item.index} processed - {item.data} at {DateTime.Now:HH:mm:ss.fff}";
                lock (lockObj)
                {
                    notifications.Add((item.index, notification));
                }
                return (item.index, item.data);
            }));
        }

        // Await all payment tasks to complete.
        var results = await Task.WhenAll(tasks);

        // Verify that notifications were sent for all payment items.
        Assert.AreEqual(items.Count, notifications.Count, "All notifications should have been sent.");

        // Based on the simulated delays, the expected order of completion (and notifications) is:
        // Order 3 (500ms), Order 1 (1000ms), Order 2 (2000ms), Order 0 (3000ms).
        var expectedOrder = new List<int> { 3, 1, 2, 0 };
        var actualOrder = notifications.Select(n => n.index).ToList();
        CollectionAssert.AreEqual(expectedOrder, actualOrder, "The notifications order does not match the expected processing order.");
    }
}