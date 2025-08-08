using UnityEngine;
using System.Collections.Generic;
using MEC;


public class Player_HpComp : HpComp
{
    public override Entity owner => origin;
    Player origin;
    protected override void Awake()
    {
        base.Awake();
        origin = GetComponent<Player>();
    }
    readonly int isDeadID = Animator.StringToHash("IsDead");
    readonly ControlElement<bool> canMoveDisable = new(false, 10, true);
    readonly ControlElement<bool> canSwitchSlotDisable = new(false, 9999, true);
    readonly ControlElement<bool> canPickupDisable = new(false, 9999, true);
    protected override void OnDeath()
    {
        base.OnDeath();
        origin.anim.SetBool(isDeadID, true);
        origin.movements.canMoveControl.AddControl(canMoveDisable);
        origin.inventory.Disarm();
        origin.inventory.canSwitchSlotControl.AddControl(canSwitchSlotDisable);
        origin.inventory.canPickupControl.AddControl(canPickupDisable);
        origin.inventory.DropAll();
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
        origin.inventory.canPickupControl.RemoveControl(canPickupDisable);
        origin.inventory.ChangeEquippedSlot(0);
        if (GameManager.Instance.roundInProgress.Value)
        {
            if(GameManager.Instance.currentGameMode.Value == GameMode.FreeForAll)
            {
                StageManager_FFA stageManager = StageManager.Instance as StageManager_FFA;
                origin.movements.TeleportRpc(stageManager.spawnPoints[UnityEngine.Random.Range(0, stageManager.spawnPoints.Count)].position);
            }
            else if(GameManager.Instance.currentGameMode.Value == GameMode.TeamMatch)
            {
                StageManager_TM stageManager = StageManager.Instance as StageManager_TM;
                origin.movements.TeleportRpc((origin.team.Value == Team.TeamRed ? stageManager.redSpawn : stageManager.blueSpawn).position);
            }
        }
    }
}