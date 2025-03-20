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
/// MSTest class demonstrating asynchronous iterator usage for I-O bound operations.
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
}