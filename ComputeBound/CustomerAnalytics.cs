using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace ComputeBoundOperations.Tests
{
    // Simple customer class.
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Value { get; set; }
        // To store the computed result.
        public long ComputedScore { get; set; }
    }

    // Class that simulates a heavy, compute-bound operation.
    public static class CustomerAnalytics
    {
        /// <summary>
        /// Performs a CPU-intensive computation.
        /// </summary>
        /// <param name="value">A numeric input value.</param>
        /// <returns>A computed score.</returns>
        public static long ComputeHeavyScore(int value)
        {
            long result = 0;
            // Simulate heavy computation.
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
        /// Generates a list of dummy customers.
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
                    // Cycle customer values between 1 and 10.
                    Value = (i % 10) + 1
                });
            }
            return customers;
        }

        /// <summary>
        /// Uses PLINQ to perform the heavy computation and aggregate the results.
        /// </summary>
        [TestMethod]
        public void TestPLINQAggregation()
        {
            var customers = GenerateCustomers(100);

            // PLINQ distributes the heavy computation across threads.
            long totalSum = customers.AsParallel()
                                     .Sum(c => CustomerAnalytics.ComputeHeavyScore(c.Value));
            long maxScore = customers.AsParallel()
                                     .Max(c => CustomerAnalytics.ComputeHeavyScore(c.Value));
            long minScore = customers.AsParallel()
                                     .Min(c => CustomerAnalytics.ComputeHeavyScore(c.Value));

            Assert.IsTrue(totalSum > 0,
                "Total aggregated score should be greater than 0.");
            Assert.IsTrue(maxScore > 0,
                "Maximum computed score should be greater than 0.");
            Assert.IsTrue(minScore >= 0,
                "Minimum computed score should be zero or more.");
        }

        /// <summary>
        /// Uses Parallel.For and Parallel.ForEach to perform identical heavy computations.
        /// This demonstrates task parallelism using both index-based and element-based approaches.
        /// </summary>
        [TestMethod]
        public void TestParallelForAndForEachAggregation()
        {
            // Create two separate customer lists.
            var customersFor = GenerateCustomers(100);
            var customersForEach = GenerateCustomers(100);

            // Using Parallel.For (index-based iteration)
            Parallel.For(0, customersFor.Count, i =>
            {
                customersFor[i].ComputedScore = CustomerAnalytics
                    .ComputeHeavyScore(customersFor[i].Value);
            });

            // Aggregate results from the Parallel.For operation.
            long totalSumFor = customersFor.Sum(c => c.ComputedScore);
            long maxScoreFor = customersFor.Max(c => c.ComputedScore);
            long minScoreFor = customersFor.Min(c => c.ComputedScore);

            // Using Parallel.ForEach (element-based iteration)
            Parallel.ForEach(customersForEach, customer =>
            {
                customer.ComputedScore = CustomerAnalytics
                    .ComputeHeavyScore(customer.Value);
            });

            long totalSumForEach = customersForEach.Sum(c => c.ComputedScore);
            long maxScoreForEach = customersForEach.Max(c => c.ComputedScore);
            long minScoreForEach = customersForEach.Min(c => c.ComputedScore);

            // Validate both methods yield expected results.
            Assert.IsTrue(totalSumFor > 0,
                "Parallel.For: Total aggregated score should be greater than 0.");
            Assert.IsTrue(maxScoreFor > 0,
                "Parallel.For: Maximum computed score should be greater than 0.");
            Assert.IsTrue(minScoreFor >= 0,
                "Parallel.For: Minimum computed score should be zero or more.");

            Assert.IsTrue(totalSumForEach > 0,
                "Parallel.ForEach: Total aggregated score should be greater than 0.");
            Assert.IsTrue(maxScoreForEach > 0,
                "Parallel.ForEach: Maximum computed score should be greater than 0.");
            Assert.IsTrue(minScoreForEach >= 0,
                "Parallel.ForEach: Minimum computed score should be zero or more.");

            // Confirm both approaches produced identical aggregates.
            Assert.AreEqual(totalSumFor, totalSumForEach,
                "Total sum must be equal between Parallel.For and Parallel.ForEach.");
            Assert.AreEqual(maxScoreFor, maxScoreForEach,
                "Max score must be equal between Parallel.For and Parallel.ForEach.");
            Assert.AreEqual(minScoreFor, minScoreForEach,
                "Min score must be equal between Parallel.For and Parallel.ForEach.");
        }

        /// <summary>
        /// Demonstrates exception handling using PLINQ.
        /// This test deliberately throws an exception for a specific customer to simulate an error.
        /// </summary>
        [TestMethod]
        public void TestPLINQExceptionHandling()
        {
            var customers = GenerateCustomers(100);

            try
            {
                // PLINQ distributes the heavy computation across threads.
                // We simulate an exception when processing the customer with Id == 50.
                long totalSum = customers.AsParallel().Sum(c =>
                {
                    if (c.Id == 50)
                        throw new InvalidOperationException(
                            "Simulated exception in PLINQ");
                    return CustomerAnalytics.ComputeHeavyScore(c.Value);
                });

                Assert.Fail("Expected exception was not thrown in PLINQ.");
            }
            catch (AggregateException aggEx)
            {
                // Verify that the simulated exception is present in the inner exceptions.
                Assert.IsTrue(aggEx.InnerExceptions.Any(e =>
                    e is InvalidOperationException &&
                    e.Message.Contains("Simulated exception in PLINQ")),
                    "PLINQ exception did not match expected.");
            }
        }

        /// <summary>
        /// Demonstrates exception handling using Parallel.For and Parallel.ForEach.
        /// This test simulates errors by throwing exceptions for specific customer IDs.
        /// </summary>
        [TestMethod]
        public void TestParallelExceptionHandling()
        {
            // --- Parallel.For exception handling ---
            var customersFor = GenerateCustomers(100);
            AggregateException forEx = null;
            try
            {
                // Using Parallel.For (index-based iteration)
                Parallel.For(0, customersFor.Count, i =>
                {
                    if (customersFor[i].Id == 25)
                        throw new InvalidOperationException(
                            "Simulated exception in Parallel.For");
                    customersFor[i].ComputedScore = CustomerAnalytics
                        .ComputeHeavyScore(customersFor[i].Value);
                });
                Assert.Fail("Expected exception was not thrown in Parallel.For.");
            }
            catch (AggregateException aggEx)
            {
                forEx = aggEx;
            }
            Assert.IsNotNull(forEx,
                "AggregateException was not thrown for Parallel.For.");
            Assert.IsTrue(forEx.InnerExceptions.Any(e =>
                e is InvalidOperationException &&
                e.Message.Contains("Simulated exception in Parallel.For")),
                "Parallel.For exception did not match expected.");

            // --- Parallel.ForEach exception handling ---
            var customersForEach = GenerateCustomers(100);
            AggregateException forEachEx = null;
            try
            {
                // Using Parallel.ForEach (element-based iteration)
                Parallel.ForEach(customersForEach, customer =>
                {
                    if (customer.Id == 75)
                        throw new InvalidOperationException(
                            "Simulated exception in Parallel.ForEach");
                    customer.ComputedScore = CustomerAnalytics
                        .ComputeHeavyScore(customer.Value);
                });
                Assert.Fail("Expected exception was not thrown in Parallel.ForEach.");
            }
            catch (AggregateException aggEx)
            {
                forEachEx = aggEx;
            }
            Assert.IsNotNull(forEachEx,
                "AggregateException was not thrown for Parallel.ForEach.");
            Assert.IsTrue(forEachEx.InnerExceptions.Any(e =>
                e is InvalidOperationException &&
                e.Message.Contains("Simulated exception in Parallel.ForEach")),
                "Parallel.ForEach exception did not match expected.");
        }
    }
}