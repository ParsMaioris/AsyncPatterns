using ThreadBound.Compute.Services;

namespace ThreadBound.Compute;

[TestClass]
public class PLINQScoringTests
{
    [TestMethod]
    public void ShouldComputeScoresUsingPLINQ()
    {
        var customers = CustomerFactory.Build(100);
        var total = customers.AsParallel().Sum(c => ComputeEngine.EvaluateScore(c.Value));
        var max = customers.AsParallel().Max(c => ComputeEngine.EvaluateScore(c.Value));
        var min = customers.AsParallel().Min(c => ComputeEngine.EvaluateScore(c.Value));
        Assert.IsTrue(total > 0);
        Assert.IsTrue(max > 0);
        Assert.IsTrue(min >= 0);
    }

    [TestMethod]
    public void ShouldHandlePLINQExceptions()
    {
        var customers = CustomerFactory.Build(100);
        try
        {
            var sum = customers.AsParallel().Sum(c =>
            {
                if (c.Id == 50) throw new InvalidOperationException("Simulated exception in PLINQ");
                return ComputeEngine.EvaluateScore(c.Value);
            });
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