using ThreadBound.IO.Services;

namespace ThreadBound.IO;

[TestClass]
public class AsyncIteratorTests
{
    private static readonly List<(int index, string note)> _notifications = new();
    private static readonly object _sync = new();

    [TestMethod]
    public async Task ShouldProcessPaymentsInExpectedOrder()
    {
        _notifications.Clear();
        var payments = await CollectPaymentsAsync();

        Assert.AreEqual(4, payments.Count);

        var expected = new List<int> { 3, 1, 2, 0 };

        var actual = payments.Select(p => p.index).ToList();

        CollectionAssert.AreEqual(expected, actual);
        Assert.AreEqual(4, _notifications.Count);
    }

    private static async Task<List<(int index, string data)>> CollectPaymentsAsync()
    {
        var results = new List<(int index, string data)>();
        await foreach (var item in AsyncIteratorPaymentService.GetPayments())
        {
            results.Add(item);
            var note = $"Notification: Payment {item.index} processed - {item.data}";
            lock (_sync)
            {
                _notifications.Add((item.index, note));
            }
        }
        return results;
    }

    [TestMethod]
    public async Task ShouldHandleIteratorExceptions()
    {
        try
        {
            var list = new List<(int, string)>();
            await foreach (var entry in AsyncIteratorPaymentService.GetPaymentsWithException())
            {
                list.Add(entry);
                var note = $"Notification: Payment {entry.index} processed - {entry.data}";
                lock (_sync)
                {
                    _notifications.Add((entry.index, note));
                }
            }

            Assert.Fail("Expected exception was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Simulated exception in async iterator", ex.Message);
        }
    }
}