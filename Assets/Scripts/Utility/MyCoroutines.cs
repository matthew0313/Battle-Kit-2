using System;
using System.Collections.Generic;
using MEC;

public static class MyCoroutines
{
    public static IEnumerator<float> WaitThen(float seconds, Action action)
    {
        yield return Timing.WaitForSeconds(seconds);
        action?.Invoke();
    }
}