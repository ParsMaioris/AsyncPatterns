namespace Locks;

public class ReadWriteLockDemo
{
    // Shared MultiValueDictionary instance.
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
                // Exception is caught and ignored.
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
    }

    static void WorkerRead()
    {
        // Reduced loop count for demo purposes.
        for (int i = 0; i < 100000; i++)
        {
            int key = i % 10;
            rwLock.EnterReadLock();
            try
            {
                var values = dict.Get(key);
                // Take a snapshot to simulate processing.
                var snapshot = new List<int>(values);
                foreach (var v in snapshot)
                {
                    // Process value (no-op).
                }
            }
            catch (KeyNotFoundException)
            {
                // Exception is caught and ignored.
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
        Task readTask = Task.Run(() => WorkerRead());

        Task.WaitAll(addTask, removeTask, readTask);
    }
}

[TestClass]
public class ReadWriteLockDemoTests
{
    /// <summary>
    /// Verifies that concurrent operations in ReadWriteLockDemo.Run() complete successfully.
    /// This test uses the internal MultiValueDictionary as implemented in your demo.
    /// </summary>
    [TestMethod]
    public void TestConcurrentOperationsCompletesSuccessfully()
    {
        // Act: Call the Run method. Unhandled exceptions will fail the test.
        ReadWriteLockDemo.Run();
        // If we reach here, the operations have completed.
        Assert.IsTrue(true, "Concurrent operations completed successfully.");
    }

    /// <summary>
    /// Demonstrates exception handling within a write-lock section using the internal MultiValueDictionary.
    /// This test forces a KeyNotFoundException by attempting to remove a value from a non-existent key,
    /// ensuring the exception is caught and the lock is properly released.
    /// </summary>
    [TestMethod]
    public void TestWriteLockExceptionHandlingWithMultiValueDictionary()
    {
        var rwLock = new ReaderWriterLockSlim();
        // Create an instance of the internal MultiValueDictionary.
        var dict = new MultiValueDictionary<int, int>();

        bool exceptionCaught = false;
        try
        {
            rwLock.EnterWriteLock();
            try
            {
                // Add an entry for key 1.
                dict.Add(1, 100);
                // Attempt to remove a value from a key that doesn't exist (key 2).
                dict.Remove(2, 200);
            }
            finally
            {
                rwLock.ExitWriteLock();
            }
        }
        catch (KeyNotFoundException)
        {
            exceptionCaught = true;
        }

        Assert.IsTrue(exceptionCaught, "Expected KeyNotFoundException was not thrown.");
        Assert.IsFalse(rwLock.IsWriteLockHeld, "Write lock should be released after exception handling.");
    }
}
