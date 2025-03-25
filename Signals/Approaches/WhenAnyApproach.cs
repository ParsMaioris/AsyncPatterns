namespace ThreadBound.Signals.Approaches;

public static class WhenAnyApproach
{
    public static async Task<List<int>> RunTasksAsync()
    {
        var tasks = new List<Task<int>>();
        var delays = new[] { 1500, 500, 1200, 300 };

        for (int i = 0; i < delays.Length; i++)
        {
            var index = i;
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(delays[index]);
                return index;
            }));
        }

        var order = new List<int>();
        while (tasks.Any())
        {
            var finished = await Task.WhenAny(tasks);
            tasks.Remove(finished);
            order.Add(await finished);
        }

        return order;
    }
}
