using ThreadBound.Signals.Approaches;

namespace ThreadBound.Signals;

[TestClass]
public class SignalDemo
{
    [TestMethod]
    public async Task ShouldReturnCompletionOrderUsingSignals()
    {
        var order = await SignalApproach.RunTasksAsync();
        Assert.AreEqual(4, order.Count);
        Assert.AreEqual(4, order.Distinct().Count());
    }
}
