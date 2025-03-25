using ThreadBound.Locks.Collections;

namespace ThreadBound.Locks.Approaches;

public static class RegularLock
{
    static readonly MultiValueDictionary<int, int> dict = new();
    static readonly object syncObj = new();

    static void Add()
    {
        for (int i = 0; i < 10000; i++)
        {
            var key = i % 10;
            lock (syncObj)
            {
                dict.Add(key, i);
            }
        }
    }

    static void Remove()
    {
        for (int i = 0; i < 10000; i++)
        {
            var key = i % 10;
            try
            {
                lock (syncObj)
                {
                    dict.Remove(key, i);
                }
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }

    static void Read()
    {
        for (int i = 0; i < 10000; i++)
        {
            var key = i % 10;
            var snapshot = new HashSet<int>();
            try
            {
                lock (syncObj)
                {
                    foreach (var val in dict.Get(key))
                    {
                        snapshot.Add(val);
                    }
                }
                foreach (var x in snapshot)
                {
                }
            }
            catch (KeyNotFoundException)
            {
            }
        }
    }

    public static void Execute()
    {
        var addTask = Task.Run(() => Add());
        var removeTask = Task.Run(() => Remove());
        var readTask = Task.Run(() => Read());
        Task.WaitAll(addTask, removeTask, readTask);
    }
}
