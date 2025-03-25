namespace ThreadBound.IO.Iterators;

public sealed class PaymentAsyncEnumerator : IAsyncEnumerator<(int index, string data)>
{
    private readonly List<Task<(int index, string data)>> _tasks;
    private (int index, string data) _current;

    public PaymentAsyncEnumerator(List<Task<(int index, string data)>> tasks)
    {
        _tasks = tasks;
    }

    public (int index, string data) Current => _current;

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask<bool> MoveNextAsync()
    {
        if (_tasks.Count == 0) return false;
        var finished = await Task.WhenAny(_tasks);
        _tasks.Remove(finished);
        _current = await finished;
        return true;
    }
}

