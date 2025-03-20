namespace IoBoundOperations
{
    class CustomerPayments
    {
        // Asynchronous iterator that simulates processing payment data
        public static async IAsyncEnumerable<(int index, string data)> GetDataAsync()
        {
            var items = new List<(int index, int delay, string data)>
            {
                (0, 3000, "Payment processed for Order 0"),
                (1, 1000, "Payment processed for Order 1"),
                (2, 2000, "Payment processed for Order 2"),
                (3, 500,  "Payment processed for Order 3")
            };

            var tasks = new List<Task<(int index, string data)>>();
            foreach (var item in items)
            {
                tasks.Add(Task.Run(async () =>
                {
                    await Task.Delay(item.delay);
                    return (item.index, item.data);
                }));
            }

            while (tasks.Count > 0)
            {
                var finishedTask = await Task.WhenAny(tasks);
                tasks.Remove(finishedTask);
                yield return await finishedTask;
            }
        }

        // Existing AppRunner for reference
        public static async Task AppRunner()
        {
            Console.WriteLine("Running CustomerPayments AppRunner...\n");
            await foreach (var result in GetDataAsync())
            {
                Console.WriteLine($"[Payment] Index: {result.index}, Data: {result.data}, Time: {DateTime.Now:HH:mm:ss.fff}");
            }

            // Optional: existing demonstration of parallel LINQ (if needed)
            "abcdef".AsParallel().Select(c => char.ToUpper(c)).ForAll(Console.WriteLine);
        }
    }

    // New service that simulates notifying a customer via email for each payment processed.
    class NotificationService
    {
        public async Task NotifyCustomerAsync()
        {
            Console.WriteLine("Starting Notification Service...\n");
            await foreach (var result in CustomerPayments.GetDataAsync())
            {
                // Simulated email notification
                Console.WriteLine($"[Notification] Email sent: Payment #{result.index} processed - {result.data} at {DateTime.Now:HH:mm:ss.fff}");
            }
        }
    }

    class Program
    {
        static async Task Main(string[] args)
        {
            // Uncomment the following line to run the original CustomerPayments AppRunner
            // await CustomerPayments.AppRunner();

            // Run the notification service that calls the asynchronous iterator
            var notifier = new NotificationService();
            await notifier.NotifyCustomerAsync();

            Console.WriteLine("\nPress Enter to exit.");
            Console.ReadLine();
        }
    }
}