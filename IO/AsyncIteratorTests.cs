using System;
using ThreadBound.IO.Services;

namespace ThreadBound.IO;

[TestClass]
public class AsyncIteratorTests
{
    [TestMethod]
    public async Task ShouldProcessPaymentsInExpectedOrder()
    {
        var payments = await CollectPaymentsAsync();

        Assert.AreEqual(4, payments.Count, "Expected exactly four payments to be processed.");

        var expectedOrder = new List<int> { 3, 1, 2, 0 };
        var actualOrder = payments.Select(payment => payment.index).ToList();
        CollectionAssert.AreEqual(expectedOrder, actualOrder, "The payment processing order is incorrect.");
    }

    private static async Task<List<(int index, string data)>> CollectPaymentsAsync()
    {
        var collectedPayments = new List<(int index, string data)>();
        await foreach (var payment in AsyncIteratorPaymentService.GetPayments())
        {
            collectedPayments.Add(payment);
        }
        return collectedPayments;
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