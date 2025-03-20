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

// -----------------------
// Unit Tests (namespace Locks.Tests)
// -----------------------
[TestClass]
public class RegularLocksDemoTests
{
    /// <summary>
    /// Verifies that concurrent operations in RegularLocksDemo.Run() complete successfully.
    /// This test exercises your demo, which uses the internal MultiValueDictionary.
    /// </summary>
    [TestMethod]
    public void TestRegularLocksConcurrentOperationsCompletesSuccessfully()
    {
        // Act: Run the demo. If an unhandled exception occurs, the test will fail.
        RegularLocksDemo.Run();
        Assert.IsTrue(true, "Concurrent operations completed successfully.");
    }

    /// <summary>
    /// Demonstrates exception handling using the internal MultiValueDictionary with a regular lock.
    /// The test attempts to remove a value for a non-existent key to force a KeyNotFoundException.
    /// </summary>
    [TestMethod]
    public void TestRegularLocksExceptionHandlingWithMultiValueDictionary()
    {
        object localLocker = new object();
        var localDict = new MultiValueDictionary<int, int>();
        bool exceptionCaught = false;

        try
        {
            lock (localLocker)
            {
                // Attempt to remove from a key that doesn't exist.
                localDict.Remove(2, 200);
            }
        }
        catch (KeyNotFoundException)
        {
            exceptionCaught = true;
        }

        Assert.IsTrue(exceptionCaught, "Expected KeyNotFoundException was not thrown.");
    }
}