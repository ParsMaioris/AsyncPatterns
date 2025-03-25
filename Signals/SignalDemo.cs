using ThreadBound.Signals.Approaches;

namespace ThreadBound.Signals;

[TestClass]
public class SignalDemo
{
    [TestMethod]
    public void ShouldReturnCompletionOrderUsingSignals()
    {
        var order = SignalApproach.RunTasks();

        Assert.AreEqual(4, order.Count);
        Assert.AreEqual(4, order.Distinct().Count());
    }
}
