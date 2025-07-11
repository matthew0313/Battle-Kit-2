using UnityEngine;
using System.Collections.Generic;
using MEC;


public class Player_HpComp : HpComp
{
    Player origin;
    protected override void Awake()
    {
        base.Awake();
        origin = GetComponent<Player>();
    }
    readonly int isDeadID = Animator.StringToHash("IsDead");
    protected override void OnDeath()
    {
        base.OnDeath();
        origin.anim.SetBool(isDeadID, true);
        Timing.RunCoroutine(ReviveWait());
    }
    IEnumerator<float> ReviveWait()
    {
        yield return Timing.WaitForSeconds(3.0f);
        Revive();
    }
    protected override void OnRevive()
    {
        base.OnRevive();
        origin.anim.SetBool(isDeadID, false);
    }
    public override bool IsEnemy(Player other)
    {
        return origin.IsEnemy(other);
    }
}