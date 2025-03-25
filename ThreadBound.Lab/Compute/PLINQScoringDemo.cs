using ThreadBound.Compute.Services;

namespace ThreadBound.Compute;

[TestClass]
public class PLINQScoringDemo
{
    [TestMethod]
    public void ShouldComputeScoresUsingPLINQ()
    {
        var customers = CustomerFactory.Build(100);
        var finalStats = customers
            .AsParallel()
            .Aggregate(
                () => (sum: 0L, max: long.MinValue, min: long.MaxValue),
                (acc, customer) =>
                {
                    var score = ComputeEngine.EvaluateScore(customer.Value);
                    acc.sum += score;
                    if (score > acc.max) acc.max = score;
                    if (score < acc.min) acc.min = score;
                    return acc;
                },
                (acc1, acc2) =>
                {
                    var combinedSum = acc1.sum + acc2.sum;
                    var combinedMax = acc1.max > acc2.max ? acc1.max : acc2.max;
                    var combinedMin = acc1.min < acc2.min ? acc1.min : acc2.min;
                    return (combinedSum, combinedMax, combinedMin);
                },
                result => result
            );

        Assert.IsTrue(finalStats.sum > 0);
        Assert.IsTrue(finalStats.max > 0);
        Assert.IsTrue(finalStats.min >= 0);
    }

    [TestMethod]
    public void ShouldHandlePLINQExceptions()
    {
        var customers = CustomerFactory.Build(100);
        try
        {
            var result = customers
                .AsParallel()
                .Aggregate(
                    () => (sum: 0L, max: long.MinValue, min: long.MaxValue),
                    (acc, customer) =>
                    {
                        if (customer.Id == 50)
                            throw new InvalidOperationException("Simulated exception in PLINQ");
                        var score = ComputeEngine.EvaluateScore(customer.Value);
                        acc.sum += score;
                        if (score > acc.max) acc.max = score;
                        if (score < acc.min) acc.min = score;
                        return acc;
                    },
                    (acc1, acc2) =>
                    {
                        var combinedSum = acc1.sum + acc2.sum;
                        var combinedMax = acc1.max > acc2.max ? acc1.max : acc2.max;
                        var combinedMin = acc1.min < acc2.min ? acc1.min : acc2.min;
                        return (combinedSum, combinedMax, combinedMin);
                    },
                    final => final
                );

            Assert.Fail();
        }
        catch (AggregateException ex)
        {
            Assert.IsTrue(
                ex.InnerExceptions.Any(e =>
                    e is InvalidOperationException &&
                    e.Message.Contains("Simulated exception in PLINQ")));
        }
    }
}