namespace ComputeBoundOperations.Tests;

// A simple customer class.
public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; }
    public int Value { get; set; }
    // Will hold the computed result from our heavy operation.
    public long ComputedScore { get; set; }
}

// A static class that simulates a compute-bound operation.
public static class CustomerAnalytics
{
    /// <summary>
    /// Simulates heavy computation by running a loop.
    /// </summary>
    /// <param name="value">An input value (e.g. customer value).</param>
    /// <returns>A computed score based on the input.</returns>
    public static long ComputeHeavyScore(int value)
    {
        long result = 0;
        // Run a loop to simulate CPU-bound work.
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
    /// <summary>
    /// Helper method to generate a list of dummy customers.
    /// </summary>
    private List<Customer> GenerateCustomers(int count)
    {
        var customers = new List<Customer>();
        for (int i = 0; i < count; i++)
        {
            customers.Add(new Customer
            {
                Id = i,
                Name = $"Customer_{i}",
                // Cycle values between 1 and 10.
                Value = (i % 10) + 1
            });
        }
        return customers;
    }

    /// <summary>
    /// Demonstrates using Parallel.For to process customers by index.
    /// </summary>
    [TestMethod]
    public void TestParallelForComputesScores()
    {
        var customers = GenerateCustomers(100);

        // Process each customer in parallel by index.
        Parallel.For(0, customers.Count, i =>
        {
            customers[i].ComputedScore = CustomerAnalytics.ComputeHeavyScore(customers[i].Value);
        });

        // Assert that each computed score is greater than 0.
        foreach (var customer in customers)
        {
            Assert.IsTrue(customer.ComputedScore > 0, $"Customer {customer.Id} has an invalid computed score.");
        }
    }

    /// <summary>
    /// Demonstrates using Parallel.ForEach to process each customer.
    /// </summary>
    [TestMethod]
    public void TestParallelForEachComputesScores()
    {
        var customers = GenerateCustomers(100);

        // Process each customer in parallel using ForEach.
        Parallel.ForEach(customers, customer =>
        {
            customer.ComputedScore = CustomerAnalytics.ComputeHeavyScore(customer.Value);
        });

        // Verify each customer’s computed score.
        foreach (var customer in customers)
        {
            Assert.IsTrue(customer.ComputedScore > 0, $"Customer {customer.Id} did not compute a valid score.");
        }
    }

    /// <summary>
    /// Demonstrates using PLINQ to aggregate computed scores from all customers.
    /// </summary>
    [TestMethod]
    public void TestPLINQAggregation()
    {
        var customers = GenerateCustomers(100);

        // Use PLINQ to compute the total aggregated score.
        long totalScore = customers
            .AsParallel()
            .Sum(c => CustomerAnalytics.ComputeHeavyScore(c.Value));

        Assert.IsTrue(totalScore > 0, "The total aggregated score should be greater than 0.");
    }

    /// <summary>
    /// Demonstrates using Parallel.Invoke to run multiple operations concurrently.
    /// </summary>
    [TestMethod]
    public void TestParallelInvokeForMultipleOperations()
    {
        var customers = GenerateCustomers(100);
        long sum = 0, max = 0, min = long.MaxValue;
        object lockObj = new object();

        // Run three operations concurrently: summing, finding max, and finding min.
        Parallel.Invoke(
            () =>
            {
                // Calculate the sum of computed scores.
                long localSum = 0;
                foreach (var customer in customers)
                {
                    localSum += CustomerAnalytics.ComputeHeavyScore(customer.Value);
                }
                lock (lockObj)
                {
                    sum = localSum;
                }
            },
            () =>
            {
                // Calculate the maximum computed score.
                long localMax = customers
                    .AsParallel()
                    .Max(c => CustomerAnalytics.ComputeHeavyScore(c.Value));
                lock (lockObj)
                {
                    max = localMax;
                }
            },
            () =>
            {
                // Calculate the minimum computed score.
                long localMin = customers
                    .AsParallel()
                    .Min(c => CustomerAnalytics.ComputeHeavyScore(c.Value));
                lock (lockObj)
                {
                    min = localMin;
                }
            }
        );

        Assert.IsTrue(sum > 0, "The sum of computed scores should be positive.");
        Assert.IsTrue(max > 0, "The maximum computed score should be positive.");
        Assert.IsTrue(min >= 0, "The minimum computed score should be zero or more.");
    }
}