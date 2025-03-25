using System;

namespace ThreadBound.Compute.Services;

public static class ComputeEngine
{
    public static long EvaluateScore(int value)
    {
        long result = 0;
        for (int i = 1; i <= 100000; i++)
        {
            result += value % (i + 1);
        }
        return result;
    }
}

