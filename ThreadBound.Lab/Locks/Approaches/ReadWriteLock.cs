using ThreadBound.Locks.Collections;

namespace ThreadBound.Locks.Approaches;

public static class ReadWriteLock
{
    static readonly MultiValueDictionary<int, int> dict = new();
    static readonly ReaderWriterLockSlim rwLock = new();

    static void Add()
    {
        for (int i = 0; i < 100; i++)
        {
            var key = i % 10;
            rwLock.EnterWriteLock();
            try
            {
                dict.Add(key, i);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }

    static void Remove()
    {
        for (int i = 0; i < 100; i++)
        {
            var key = i % 10;
            rwLock.EnterWriteLock();
            try
            {
                dict.Remove(key, i);
            }
            catch (KeyNotFoundException)
            {
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }

    static void Read()
    {
        for (int i = 0; i < 100000; i++)
        {
            var key = i % 10;
            rwLock.EnterReadLock();
            try
            {
                var values = dict.Get(key);
                var snapshot = new List<int>(values);
                foreach (var v in snapshot)
                {
                }
            }
            catch (KeyNotFoundException)
            {
            }
            finally
            {
                if (rwLock.IsReadLockHeld) rwLock.ExitReadLock();
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
