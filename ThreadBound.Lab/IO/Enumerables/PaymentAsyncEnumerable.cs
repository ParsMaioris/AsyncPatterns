using ThreadBound.IO.Iterators;

namespace ThreadBound.IO.Enumerables;

public sealed class PaymentAsyncEnumerable : IAsyncEnumerable<(int index, string data)>
{
    readonly List<Task<(int, string)>> _tasks;

    public PaymentAsyncEnumerable(List<Task<(int, string)>> tasks)
    {
        _tasks = tasks;
    }

    public IAsyncEnumerator<(int index, string data)> GetAsyncEnumerator(CancellationToken token = default)
    {
        return new PaymentAsyncEnumerator(_tasks);
    }
}
