namespace ThreadBound.Signals.Approaches;

public static class SignalApproach
{
    public static async Task<List<int>> RunTasksAsync()
    {
        var signals = new List<(ManualResetEventSlim handle, int originalIndex)>();
        var tasks = new List<Task>();
        var delays = new[] { 1500, 500, 1200, 300 };
        var order = new List<int>();

        for (int i = 0; i < delays.Length; i++)
        {
            var index = i;
            var signal = new ManualResetEventSlim(false);
            signals.Add((signal, index));
            tasks.Add(Task.Run(async () =>
            {
                await Task.Delay(delays[index]);
                signal.Set();
            }));
        }

        _ = Task.WhenAll(tasks);

        while (signals.Any())
        {
            var waitHandles = signals.Select(s => s.handle.WaitHandle).ToArray();
            var triggered = WaitHandle.WaitAny(waitHandles);
            var chosen = signals[triggered];
            signals.RemoveAt(triggered);
            chosen.handle.Dispose();
            order.Add(chosen.originalIndex);
        }

        return order;
    }
}
