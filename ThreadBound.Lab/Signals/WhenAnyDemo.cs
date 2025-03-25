using ThreadBound.Signals.Approaches;

namespace ThreadBound.Signals;

[TestClass]
public class WhenAnyDemo
{
    [TestMethod]
    public async Task ShouldReturnCompletionOrderUsingWhenAny()
    {
        var order = await WhenAnyApproach.RunTasksAsync();

        Assert.AreEqual(4, order.Count);

        var distinct = order.Distinct().Count();

        Assert.AreEqual(4, distinct);
    }
}
