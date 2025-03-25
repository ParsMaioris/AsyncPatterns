using ThreadBound.Locks.Approaches;
using ThreadBound.Locks.Collections;

namespace ThreadBound.Locks;

[TestClass]
public class RegularLockTests
{
    [TestMethod]
    public void ShouldCompleteInRegularLock()
    {
        RegularLock.Execute();
        Assert.IsTrue(true);
    }

    [TestMethod]
    public void ShouldHandleRegularLockException()
    {
        var syncObj = new object();
        var localDict = new MultiValueDictionary<int, int>();
        bool caught = false;
        try
        {
            lock (syncObj)
            {
                localDict.Remove(2, 200);
            }
        }
        catch (KeyNotFoundException)
        {
            caught = true;
        }
        Assert.IsTrue(caught);
    }
}
