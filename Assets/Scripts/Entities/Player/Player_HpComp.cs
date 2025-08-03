using UnityEngine;
using System.Collections.Generic;
using MEC;


public class Player_HpComp : HpComp
{
    public override Entity self => origin;
    Player origin;
    protected override void Awake()
    {
        base.Awake();
        origin = GetComponent<Player>();
    }
    readonly int isDeadID = Animator.StringToHash("IsDead");
    readonly ControlElement<bool> canMoveDisable = new(false, 10, true);
    readonly ControlElement<bool> canSwitchSlotDisable = new(false, 9999, true);
    protected override void OnDeath()
    {
        base.OnDeath();
        origin.anim.SetBool(isDeadID, true);
        origin.movements.canMoveControl.AddControl(canMoveDisable);
        origin.inventory.Disarm();
        origin.inventory.canSwitchSlotControl.AddControl(canSwitchSlotDisable);
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
        origin.movements.canMoveControl.RemoveControl(canMoveDisable);
        origin.inventory.canSwitchSlotControl.RemoveControl(canSwitchSlotDisable);
        origin.inventory.ChangeEquippedSlot(0);
    }
}