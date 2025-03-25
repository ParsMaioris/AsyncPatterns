using System;
using ThreadBound.IO.Services;

namespace ThreadBound.IO;

[TestClass]
public class AsyncIteratorNotificationTests
{
    [TestMethod]
    public async Task ShouldProcessPaymentsAsyncIterator()
    {
        var output = new List<(int index, string data)>();
        await foreach (var entry in AsyncIteratorPaymentService.GetPayments())
        {
            output.Add(entry);
        }
        Assert.AreEqual(4, output.Count);
        var expectedCompletionOrder = new List<int> { 3, 1, 2, 0 };
        var actualOrder = output.Select(e => e.index).ToList();
        CollectionAssert.AreEqual(expectedCompletionOrder, actualOrder);
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
            }
            Assert.Fail("Expected exception was not thrown");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Simulated exception in async iterator", ex.Message);
        }
    }
}