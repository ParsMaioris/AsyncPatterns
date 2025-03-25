using ThreadBound.IO.Services;

namespace ThreadBound.IO;

[TestClass]
public class TaskBasedAsyncTests
{
    [TestMethod]
    public async Task ShouldReceiveInlineNotifications()
    {
        await AwaitPatternPaymentService.ProcessPaymentsAsync();

        var notes = AwaitPatternPaymentService.Notifications;

        Assert.AreEqual(4, notes.Count);

        var expected = new List<int> { 3, 1, 2, 0 };

        var actual = notes.Select(n => n.index).OrderBy(x => x).ToList();
        CollectionAssert.AreEqual(expected.OrderBy(x => x).ToList(), actual);
    }

    [TestMethod]
    public async Task ShouldThrowExceptionInInlineNotification()
    {
        try
        {
            await AwaitPatternPaymentService.ProcessPaymentsWithException();

            Assert.Fail("Expected exception was not thrown.");
        }
        catch (InvalidOperationException ex)
        {
            Assert.AreEqual("Simulated exception in inline notification", ex.Message);
        }

        var notes = AwaitPatternPaymentService.Notifications;
        Assert.IsTrue(notes.Count < 4);
    }
}