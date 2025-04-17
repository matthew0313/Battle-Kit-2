using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Wooden Bow", menuName = "Scriptables/Items/Unstackables/Weapons/Bows/Wooden Bow")]
public class WoodenBow_ItemData : BowItemData
{
    [Header("Wooden Bow")]
    [SerializeField] WoodenBow m_weaponPrefab;
    public override Weapon weaponPrefab => m_weaponPrefab;

    [Header("Empower")]
    [SerializeField] Sprite m_empowerIcon;
    [SerializeField] float m_empowerCooldown = 10.0f;
    [SerializeField] int m_empowerShotCount = 3;
    [SerializeField] float m_empowerShotInterval = 0.1f;
    public Sprite empowerIcon => m_empowerIcon;
    public float empowerCooldown => m_empowerCooldown;
    public int empowerShotCount => m_empowerShotCount;
    public float empowerShotInterval => m_empowerShotInterval;

    [Header("Roll")]
    [SerializeField] Sprite m_rollIcon;
    [SerializeField] float m_rollCooldown = 3.0f;
    [SerializeField] float m_rollDistance = 5.0f;
    [SerializeField] float m_rollSpeed = 15.0f;
    public Sprite rollIcon => m_rollIcon;
    public float rollCooldown => m_rollCooldown;
    public float rollDistance => m_rollDistance;
    public float rollSpeed => m_rollSpeed;
    public override Item Create() => new WoodenBow_Item(this);
}
public class WoodenBow_Item : BowItem, ISkill1User, ISkill2User
{
    new public readonly WoodenBow_ItemData data;
    new protected WoodenBow weapon => base.weapon as WoodenBow;
    public WoodenBow_Item(WoodenBow_ItemData data) : base(data)
    {
        this.data = data;
    }
    float empowerCounter = 0.0f;
    bool empowered = false;
    public Sprite skill1Sprite => data.empowerIcon;
    public bool canUseSkill1 => !empowered && empowerCounter <= 0.0f;
    public float skill1CooldownBarFill => empowerCounter / data.empowerCooldown;
    public void Skill1Use(Player wielder)
    {
        if (!canUseSkill1) return;
        empowerCounter = data.empowerCooldown;
        empowered = true;
        weapon.empowered.Value = true;
    }
    float rollCounter = 0.0f;
    public Sprite skill2Sprite => data.rollIcon;
    public bool canUseSkill2 => InputManager.Instance.MovementInput().magnitude > 0.1f && rollCounter <= 0.0f;
    public float skill2CooldownBarFill => rollCounter / data.rollCooldown;
    public void Skill2Use(Player wielder)
    {
        if(!canUseSkill2) return;
        rolling = Timing.RunCoroutine(Rolling(wielder));
        rollCounter = data.rollCooldown;
    }
    public override void Update()
    {
        base.Update();
        if (!empowered) empowerCounter = Mathf.Max(0.0f, empowerCounter - Time.deltaTime);
        rollCounter = Mathf.Max(0.0f, rollCounter - Time.deltaTime);
    }
    CoroutineHandle empowerShot, rolling;
    protected override void Fire(Player wielder)
    {
        if(!empowered) base.Fire(wielder);
        else
        {
            empowered = false;
            weapon.empowered.Value = false;
            empowerShot = Timing.RunCoroutine(EmpowerShot(wielder));
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        Timing.KillCoroutines(empowerShot);
        empowered = false;
        weapon.empowered.Value = false;
    }
    IEnumerator<float> EmpowerShot(Player wielder)
    {
        for(int i = 0; i < data.empowerShotCount; i++)
        {
            base.Fire(wielder);
            yield return Timing.WaitForSeconds(data.empowerShotInterval);
        }
    }
    static ControlElement<Player_Movements> rollMovementDisable;
    IEnumerator<float> Rolling(Player wielder)
    {
        if (rollMovementDisable == null) rollMovementDisable = new(10, (movements) => movements.canMove = false, true);
        wielder.movements.canMoveControl.AddControls(rollMovementDisable);
        wielder.animations.RollRpc();

        float counter = 0.0f;
        Vector2 dir = InputManager.Instance.MovementInput().normalized;
        while (counter * data.rollSpeed < data.rollDistance)
        {
            wielder.movements.rb.linearVelocity = dir * data.rollSpeed;
            counter += Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }
        wielder.movements.canMoveControl.RemoveControl(rollMovementDisable);
    }
}