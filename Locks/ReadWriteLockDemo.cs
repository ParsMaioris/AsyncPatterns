using ThreadBound.Locks.Approaches;
using ThreadBound.Locks.Collections;

namespace ThreadBound.Locks;

[TestClass]
public class ReadWriteLockDemo
{
    [TestMethod]
    public void ShouldCompleteInReadWriteLock()
    {
        ReadWriteLock.Execute();
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void ShouldHandleWriteLockException()
    {
        var lockObject = new ReaderWriterLockSlim();
        var localDict = new MultiValueDictionary<int, int>();
        bool caught = false;

        try
        {
            lockObject.EnterWriteLock();
            try
            {
                localDict.Add(1, 100);
                localDict.Remove(2, 200);
            }
            finally
            {
                lockObject.ExitWriteLock();
            }
        }
        catch (KeyNotFoundException)
        {
            caught = true;
        }

        Assert.IsTrue(caught);
        Assert.IsFalse(lockObject.IsWriteLockHeld);
    }
}
