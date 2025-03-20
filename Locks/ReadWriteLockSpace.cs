namespace Locks;

public class ReadWriteLockDemo
{
    static MultiValueDictionary<int, int> dict = new MultiValueDictionary<int, int>();
    static ReaderWriterLockSlim rwLock = new ReaderWriterLockSlim();

    static void WorkerAdd()
    {


        for (int i = 0; i < 100; i++)
        {
            int key = i % 10;
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

    static void WorkerRemove()
    {


        for (int i = 0; i < 100; i++)
        {
            int key = i % 10;
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

    static void WorkerRead()
    {


        for (int i = 0; i < 1000000; i++)
        {
            int key = i % 10;
            try
            {
                rwLock.EnterReadLock();
                var values = dict.Get(key);
                var snapshot = new List<int>(values);

                foreach (var v in snapshot)
                {
                    //
                }
            }
            catch (KeyNotFoundException)
            {
                //
            }
            finally
            {
                if (rwLock.IsReadLockHeld)
                    rwLock.ExitReadLock();
            }
        }

    }

    public static void Run()
    {
        Task addTask = Task.Run(() => WorkerAdd());
        Task removeTask = Task.Run(() => WorkerRemove());
        Task raedTask = Task.Run(() => WorkerRead());

        Task.WaitAll(addTask, removeTask, raedTask);

        //Console.WriteLine("Concurrent operations finished. Check for unexpected exceptions or inconsistent state.");
    }
}