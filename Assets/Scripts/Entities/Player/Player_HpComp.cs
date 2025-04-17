using UnityEngine;
using System.Collections.Generic;
using MEC;


public class Player_HpComp : HpComp
{
    public void OnStart()
    {
        Init();
    }
    protected override void OnDeath()
    {
        base.OnDeath();
        Timing.RunCoroutine(ReviveWait());
    }
    IEnumerator<float> ReviveWait()
    {
        yield return Timing.WaitForSeconds(3.0f);
        Revive();
    }
}