using System;
using UnityEngine;

public static class RNG
{
    public static void UniqueNumbers(int start, int range, Span<int> outputBuffer)
    {
        int length = Math.Min(range, outputBuffer.Length);
        Span<int> tempNums = stackalloc int[range];
        for (int i = 0; i < range; ++i) tempNums[i] = start + i;

        for (int i = 1; i < length; ++i)
        {
            int j = UnityEngine.Random.Range(i, range);
            (tempNums[i], tempNums[j]) = (tempNums[j], tempNums[i]);
            outputBuffer[i] = tempNums[i];
        }
    }
}
