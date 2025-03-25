using ThreadBound.IO.Services;

namespace ThreadBound.IO;

[TestClass]
public class CustomIteratorAsyncTests
{
    static readonly List<(int index, string note)> Notifications = new();

    [TestMethod]
    public async Task ShouldProcessPaymentsWithCustomIterator()
    {
        Notifications.Clear();
        var items = new List<(int index, string data)>();
        await foreach (var payment in CustomIteratorPaymentService.GetPayments())
        {
            items.Add(payment);
            var note = $"Payment {payment.index} processed - {payment.data}";
            Notifications.Add((payment.index, note));
        }

        Assert.AreEqual(4, items.Count);

        var expectedOrder = new List<int> { 3, 1, 2, 0 };

        var actualOrder = items.Select(i => i.index).ToList();

        CollectionAssert.AreEqual(expectedOrder, actualOrder);
        Assert.AreEqual(4, Notifications.Count);
    }

    [TestMethod]
    public async Task ShouldHandleCustomIteratorExceptions()
    {
        Notifications.Clear();
        try
        {
            var items = new List<(int index, string data)>();
            await foreach (var payment in CustomIteratorPaymentService.GetPaymentsWithException())
            {
                items.Add(payment);
                var note = $"Payment {payment.index} processed - {payment.data}";
                Notifications.Add((payment.index, note));
            }
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Simulated exception in async iterator", ex.Message);
        }
        Assert.IsTrue(Notifications.Count < 4);
    }
}
