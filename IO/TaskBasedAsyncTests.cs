using System;
using ThreadBound.IO.Services;

namespace ThreadBound.IO;

[TestClass]
public class TaskBasedAsyncTests
{
    [TestMethod]
    public async Task ShouldReceiveNotificationsAfterAwait()
    {
        var notifications = new List<(int index, string note)>();
        var lockObj = new object();

        var results = await AwaitPatternPaymentService.ProcessPaymentsAsync();
        Parallel.ForEach(results, item =>
        {
            var note = $"Notification: Payment {item.index} processed - {item.data}";
            lock (lockObj)
            {
                notifications.Add((item.index, note));
            }
        });

        Assert.AreEqual(4, results.Count);
        var expected = new List<int> { 3, 1, 2, 0 };
        var actual = results.Select(r => r.index).OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(expected.OrderBy(x => x).ToList(), actual);
        Assert.AreEqual(4, notifications.Count);
    }

    [TestMethod]
    public async Task ShouldThrowExceptionInAwaitPattern()
    {
        var notifications = new List<(int index, string note)>();

        try
        {
            var results = await AwaitPatternPaymentService.ProcessPaymentsWithException();
            Parallel.ForEach(results, item =>
            {
                notifications.Add((item.index, $"Notification: Payment {item.index} processed - {item.data}"));
            });
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Simulated exception in inline notification", ex.Message);
        }

        Assert.IsTrue(notifications.Count < 4);
    }
}
