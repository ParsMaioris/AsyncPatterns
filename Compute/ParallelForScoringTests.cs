using System;
using ThreadBound.Compute.Services;

namespace ThreadBound.Compute;

[TestClass]
public class ParallelForScoringTests
{
    [TestMethod]
    public void ShouldComputeScoresUsingParallelForAndForEach()
    {
        var listFor = CustomerFactory.Build(100);
        var listForEach = CustomerFactory.Build(100);
        Parallel.For(0, listFor.Count, i =>
        {
            listFor[i].ComputedScore = ComputeEngine.EvaluateScore(listFor[i].Value);
        });
        var totalFor = listFor.Sum(c => c.ComputedScore);
        var maxFor = listFor.Max(c => c.ComputedScore);
        var minFor = listFor.Min(c => c.ComputedScore);
        Parallel.ForEach(listForEach, c =>
        {
            c.ComputedScore = ComputeEngine.EvaluateScore(c.Value);
        });
        var totalForEach = listForEach.Sum(c => c.ComputedScore);
        var maxForEach = listForEach.Max(c => c.ComputedScore);
        var minForEach = listForEach.Min(c => c.ComputedScore);
        Assert.IsTrue(totalFor > 0);
        Assert.IsTrue(maxFor > 0);
        Assert.IsTrue(minFor >= 0);
        Assert.IsTrue(totalForEach > 0);
        Assert.IsTrue(maxForEach > 0);
        Assert.IsTrue(minForEach >= 0);
        Assert.AreEqual(totalFor, totalForEach);
        Assert.AreEqual(maxFor, maxForEach);
        Assert.AreEqual(minFor, minForEach);
    }

    [TestMethod]
    public void ShouldHandleParallelExceptions()
    {
        var listFor = CustomerFactory.Build(100);
        AggregateException forEx = null;
        try
        {
            Parallel.For(0, listFor.Count, i =>
            {
                if (listFor[i].Id == 25) throw new InvalidOperationException("Simulated exception in Parallel.For");
                listFor[i].ComputedScore = ComputeEngine.EvaluateScore(listFor[i].Value);
            });
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
                e.Message.Contains("Simulated exception in Parallel.For")));
        var listForEach = CustomerFactory.Build(100);
        AggregateException forEachEx = null;
        try
        {
            Parallel.ForEach(listForEach, c =>
            {
                if (c.Id == 75) throw new InvalidOperationException("Simulated exception in Parallel.ForEach");
                c.ComputedScore = ComputeEngine.EvaluateScore(c.Value);
            });
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
                e.Message.Contains("Simulated exception in Parallel.ForEach")));
    }
}

