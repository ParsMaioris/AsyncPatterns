namespace ComputeBoundOperations;

public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
    public long ComputedScore { get; set; }
}

public static class CustomerAnalytics
{
    public static long CalculateScore(int value)
    {
        long result = 0;
        for (int i = 1; i <= 100_000; i++)
        {
            result += value % (i + 1);
        }
        return result;
    }
}

[TestClass]
public class CustomerAnalyticsTests
{
    private List<Customer> BuildCustomers(int count)
    {
        var list = new List<Customer>();
        for (int i = 0; i < count; i++)
        {
            list.Add(new Customer
            {
                Id = i,
                Name = $"Customer_{i}",
                Value = (i % 10) + 1
            });
        }
        return list;
    }

    [TestMethod]
    public void ShouldComputeScoresUsingPLINQ()
    {
        var customers = BuildCustomers(100);
        var total = customers.AsParallel().Sum(c => CustomerAnalytics.CalculateScore(c.Value));
        var max = customers.AsParallel().Max(c => CustomerAnalytics.CalculateScore(c.Value));
        var min = customers.AsParallel().Min(c => CustomerAnalytics.CalculateScore(c.Value));
        Assert.IsTrue(total > 0, "Total score should be greater than 0.");
        Assert.IsTrue(max > 0, "Max score should be greater than 0.");
        Assert.IsTrue(min >= 0, "Min score should be zero or more.");
    }

    [TestMethod]
    public void ShouldComputeScoresUsingParallelForAndForEach()
    {
        var listFor = BuildCustomers(100);
        var listForEach = BuildCustomers(100);
        Parallel.For(0, listFor.Count, i =>
        {
            listFor[i].ComputedScore = CustomerAnalytics.CalculateScore(listFor[i].Value);
        });
        var totalFor = listFor.Sum(c => c.ComputedScore);
        var maxFor = listFor.Max(c => c.ComputedScore);
        var minFor = listFor.Min(c => c.ComputedScore);
        Parallel.ForEach(listForEach, customer =>
        {
            customer.ComputedScore = CustomerAnalytics.CalculateScore(customer.Value);
        });
        var totalForEach = listForEach.Sum(c => c.ComputedScore);
        var maxForEach = listForEach.Max(c => c.ComputedScore);
        var minForEach = listForEach.Min(c => c.ComputedScore);
        Assert.IsTrue(totalFor > 0, "Parallel.For total should be greater than 0.");
        Assert.IsTrue(maxFor > 0, "Parallel.For max should be greater than 0.");
        Assert.IsTrue(minFor >= 0, "Parallel.For min should be zero or more.");
        Assert.IsTrue(totalForEach > 0, "Parallel.ForEach total should be greater than 0.");
        Assert.IsTrue(maxForEach > 0, "Parallel.ForEach max should be greater than 0.");
        Assert.IsTrue(minForEach >= 0, "Parallel.ForEach min should be zero or more.");
        Assert.AreEqual(totalFor, totalForEach, "Totals must match.");
        Assert.AreEqual(maxFor, maxForEach, "Max values must match.");
        Assert.AreEqual(minFor, minForEach, "Min values must match.");
    }

    [TestMethod]
    public void ShouldHandlePLINQExceptions()
    {
        var customers = BuildCustomers(100);
        try
        {
            var sum = customers.AsParallel().Sum(c =>
            {
                if (c.Id == 50) throw new InvalidOperationException("Simulated exception in PLINQ");
                return CustomerAnalytics.CalculateScore(c.Value);
            });
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (AggregateException ex)
        {
            Assert.IsTrue(
                ex.InnerExceptions.Any(e =>
                    e is InvalidOperationException &&
                    e.Message.Contains("Simulated exception in PLINQ")),
                "PLINQ exception mismatch."
            );
        }
    }

    [TestMethod]
    public void ShouldHandleParallelExceptions()
    {
        var listFor = BuildCustomers(100);
        AggregateException forEx = null;
        try
        {
            Parallel.For(0, listFor.Count, i =>
            {
                if (listFor[i].Id == 25) throw new InvalidOperationException("Simulated exception in Parallel.For");
                listFor[i].ComputedScore = CustomerAnalytics.CalculateScore(listFor[i].Value);
            });
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (AggregateException ex)
        {
            forEx = ex;
        }
        Assert.IsNotNull(forEx, "AggregateException was not thrown.");
        Assert.IsTrue(
            forEx.InnerExceptions.Any(e =>
                e is InvalidOperationException &&
                e.Message.Contains("Simulated exception in Parallel.For")),
            "Parallel.For exception mismatch."
        );

        var listForEach = BuildCustomers(100);
        AggregateException forEachEx = null;
        try
        {
            Parallel.ForEach(listForEach, c =>
            {
                if (c.Id == 75) throw new InvalidOperationException("Simulated exception in Parallel.ForEach");
                c.ComputedScore = CustomerAnalytics.CalculateScore(c.Value);
            });
            Assert.Fail("Expected exception was not thrown.");
        }
        catch (AggregateException ex)
        {
            forEachEx = ex;
        }
        Assert.IsNotNull(forEachEx, "AggregateException was not thrown.");
        Assert.IsTrue(
            forEachEx.InnerExceptions.Any(e =>
                e is InvalidOperationException &&
                e.Message.Contains("Simulated exception in Parallel.ForEach")),
            "Parallel.ForEach exception mismatch."
        );
    }
}