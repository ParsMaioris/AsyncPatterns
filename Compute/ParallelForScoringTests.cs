using ThreadBound.Compute.Services;

namespace ThreadBound.Compute;

[TestClass]
public class ParallelForScoringTests
{
    [TestMethod]
    public void ShouldComputeScoresUsingParallelForAndForEach()
    {
        var listFor = CustomerFactory.Build(100);
        var (sumFor, maxFor, minFor) = (0L, long.MinValue, long.MaxValue);
        var forLock = new object();

        Parallel.For(
            0,
            listFor.Count,
            () => (localSum: 0L, localMax: long.MinValue, localMin: long.MaxValue),
            (i, _, local) =>
            {
                var score = ComputeEngine.EvaluateScore(listFor[i].Value);
                local.localSum += score;
                if (score > local.localMax) local.localMax = score;
                if (score < local.localMin) local.localMin = score;
                return local;
            },
            finalLocal =>
            {
                lock (forLock)
                {
                    sumFor += finalLocal.localSum;
                    if (finalLocal.localMax > maxFor) maxFor = finalLocal.localMax;
                    if (finalLocal.localMin < minFor) minFor = finalLocal.localMin;
                }
            }
        );

        var listForEach = CustomerFactory.Build(100);
        var (sumForEach, maxForEach, minForEach) = (0L, long.MinValue, long.MaxValue);
        var forEachLock = new object();

        Parallel.ForEach(
            listForEach,
            () => (localSum: 0L, localMax: long.MinValue, localMin: long.MaxValue),
            (customer, _, local) =>
            {
                var score = ComputeEngine.EvaluateScore(customer.Value);
                local.localSum += score;
                if (score > local.localMax) local.localMax = score;
                if (score < local.localMin) local.localMin = score;
                return local;
            },
            finalLocal =>
            {
                lock (forEachLock)
                {
                    sumForEach += finalLocal.localSum;
                    if (finalLocal.localMax > maxForEach) maxForEach = finalLocal.localMax;
                    if (finalLocal.localMin < minForEach) minForEach = finalLocal.localMin;
                }
            }
        );

        Assert.IsTrue(sumFor > 0);
        Assert.IsTrue(maxFor > 0);
        Assert.IsTrue(minFor >= 0);
        Assert.IsTrue(sumForEach > 0);
        Assert.IsTrue(maxForEach > 0);
        Assert.IsTrue(minForEach >= 0);
        Assert.AreEqual(sumFor, sumForEach);
        Assert.AreEqual(maxFor, maxForEach);
        Assert.AreEqual(minFor, minForEach);
    }

    [TestMethod]
    public void ShouldHandleParallelExceptions()
    {
        var listFor = CustomerFactory.Build(100);
        AggregateException forEx = null!;
        try
        {
            Parallel.For(
                0,
                listFor.Count,
                i =>
                {
                    if (listFor[i].Id == 25)
                        throw new InvalidOperationException("Simulated exception in Parallel.For");
                    listFor[i].ComputedScore = ComputeEngine.EvaluateScore(listFor[i].Value);
                }
            );
            Assert.Fail();
        }
        catch (AggregateException ex)
        {
            forEx = ex;
        }

        Assert.IsNotNull(forEx);
        Assert.IsTrue(
            forEx.InnerExceptions.Any(e =>
                e is InvalidOperationException &&
                e.Message.Contains("Simulated exception in Parallel.For"))
        );

        var listForEach = CustomerFactory.Build(100);
        AggregateException forEachEx = null!;
        try
        {
            Parallel.ForEach(
                listForEach,
                c =>
                {
                    if (c.Id == 75)
                        throw new InvalidOperationException("Simulated exception in Parallel.ForEach");
                    c.ComputedScore = ComputeEngine.EvaluateScore(c.Value);
                }
            );
            Assert.Fail();
        }
        catch (AggregateException ex)
        {
            forEachEx = ex;
        }

        Assert.IsNotNull(forEachEx);
        Assert.IsTrue(
            forEachEx.InnerExceptions.Any(e =>
                e is InvalidOperationException &&
                e.Message.Contains("Simulated exception in Parallel.ForEach"))
        );
    }
}