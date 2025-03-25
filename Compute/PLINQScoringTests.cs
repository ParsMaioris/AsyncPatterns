using ThreadBound.Compute.Services;

namespace ThreadBound.Compute;

[TestClass]
public class PLINQScoringTests
{
    [TestMethod]
    public void ShouldComputeScoresUsingPLINQ()
    {
        var customers = CustomerFactory.Build(100);
        var stats = customers
            .AsParallel()
            .Aggregate(
                () => new ScoreAccumulator(),
                (acc, c) => acc.Accumulate(ComputeEngine.EvaluateScore(c.Value)),
                (acc1, acc2) => acc1.Combine(acc2),
                final => final
            );
        Assert.IsTrue(stats.Sum > 0);
        Assert.IsTrue(stats.Max > 0);
        Assert.IsTrue(stats.Min >= 0);
    }

    [TestMethod]
    public void ShouldHandlePLINQExceptions()
    {
        var customers = CustomerFactory.Build(100);
        try
        {
            var stats = customers
                .AsParallel()
                .Aggregate(
                    () => new ScoreAccumulator(),
                    (acc, c) =>
                    {
                        if (c.Id == 50) throw new InvalidOperationException("Simulated exception in PLINQ");
                        return acc.Accumulate(ComputeEngine.EvaluateScore(c.Value));
                    },
                    (acc1, acc2) => acc1.Combine(acc2),
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

public class ScoreAccumulator
{
    public long Sum { get; private set; }
    public long Max { get; private set; }
    public long Min { get; private set; }

    public ScoreAccumulator()
    {
        Sum = 0;
        Max = long.MinValue;
        Min = long.MaxValue;
    }

    public ScoreAccumulator Accumulate(long value)
    {
        Sum += value;
        if (value > Max) Max = value;
        if (value < Min) Min = value;
        return this;
    }

    public ScoreAccumulator Combine(ScoreAccumulator other)
    {
        Sum += other.Sum;
        if (other.Max > Max) Max = other.Max;
        if (other.Min < Min) Min = other.Min;
        return this;
    }
}