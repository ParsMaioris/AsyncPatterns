namespace Locks;

public class RegularLocksDemo
{
    static MultiValueDictionary<int, int> dict = new MultiValueDictionary<int, int>();
    static object locker = new object();
    static void WorkerAdd()
    {


        for (int i = 0; i < 10000; i++)
        {
            int key = i % 10;
            lock (locker)
            {
                dict.Add(key, i);
            }
        }
    }

    static void WorkerRemove()
    {


        for (int i = 0; i < 10000; i++)
        {
            int key = i % 10;
            try
            {
                lock (locker)
                {
                    dict.Remove(key, i);
                }
            }
            catch (KeyNotFoundException)
            {

            }

        }
    }

    static void WorkerRead()
    {


        for (int i = 0; i < 10000; i++)
        {
            int key = i % 10;
            HashSet<int> values = new HashSet<int>();
            try
            {
                lock (locker)
                {
                    foreach (var val in dict.Get(key))
                    {
                        values.Add(val);
                    }
                }
                foreach (var v in values)
                {
                    //
                }
            }
            catch (KeyNotFoundException)
            {
                //
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