using UnityEngine;
using System.Collections.Generic;
using MEC;


public class Player_HpComp : HpComp
{
    Player origin;
    public override void OnAwake()
    {
        base.OnAwake();
        origin = GetComponent<Player>();
    }
    public void OnStart()
    {
        if(IsOwner) Init();
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
    public override bool IsEnemy(Player other)
    {
        return origin.combat.Value == true && (other.alliance.Value & origin.alliance.Value) == 0;
    }
}