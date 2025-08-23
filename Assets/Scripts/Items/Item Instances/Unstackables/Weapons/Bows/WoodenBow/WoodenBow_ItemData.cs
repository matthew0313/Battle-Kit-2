using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Wooden Bow", menuName = "Scriptables/Items/Unstackables/Weapons/Bows/Wooden Bow")]
public class WoodenBow_ItemData : BowItemData
{
    [Header("Wooden Bow")]
    [SerializeField] WoodenBow m_weaponPrefab;
    [SerializeField] WoodenBow_Arrow m_arrow;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public override Arrow arrow => m_arrow;

    [Header("Empower")]
    [SerializeField] Sprite m_empowerIcon;
    [SerializeField] CooldownSource m_empowerCooldownSource;
    [SerializeField] float m_empowerCooldown = 10.0f;
    [SerializeField] int m_empowerShotCount = 3;
    [SerializeField] float m_empowerShotInterval = 0.1f;
    public Sprite empowerIcon => m_empowerIcon;
    public CooldownSource empowerCooldownSource => m_empowerCooldownSource;
    public float empowerCooldown => m_empowerCooldown;
    public int empowerShotCount => m_empowerShotCount;
    public float empowerShotInterval => m_empowerShotInterval;

    [Header("Roll")]
    [SerializeField] Sprite m_rollIcon;
    [SerializeField] CooldownSource m_rollCooldownSource;
    [SerializeField] float m_rollCooldown = 3.0f;
    [SerializeField] float m_rollDistance = 5.0f;
    [SerializeField] float m_rollSpeed = 15.0f;
    public Sprite rollIcon => m_rollIcon;
    public CooldownSource rollCooldownSource => m_rollCooldownSource;
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
    float empowerCounter => data.empowerCooldownSource.cooldownLeft;
    float empowerCooldown => data.empowerCooldownSource.cooldown;
    bool empowered = false;
    public Sprite skill1Sprite => data.empowerIcon;
    public bool canUseSkill1 => !empowered && empowerCounter <= 0.0f;
    public float skill1CooldownBarFill => empowerCounter / empowerCooldown;
    public void Skill1Use(Player wielder)
    {
        if (!canUseSkill1) return;
        wielder.cooldowns.AddCooldown(data.empowerCooldownSource, data.empowerCooldown);
        empowered = true;
        weapon.SetEmpowered(true);
    }
    float rollCounter => data.rollCooldownSource.cooldownLeft;
    float rollCooldown => data.rollCooldownSource.cooldown;
    public Sprite skill2Sprite => data.rollIcon;
    public bool canUseSkill2 => Mathf.Abs(InputManager.Instance.MovementInput()) > 0.1f && rollCounter <= 0.0f;
    public float skill2CooldownBarFill => rollCounter / rollCooldown;
    public void Skill2Use(Player wielder)
    {
        if(!canUseSkill2) return;
        wielder.cooldowns.AddCooldown(data.rollCooldownSource, data.rollCooldown);
        rolling = Timing.RunCoroutine(Rolling(wielder));
    }
    CoroutineHandle empowerShot, rolling;
    protected override void Fire(Player wielder)
    {
        if(!empowered) base.Fire(wielder);
        else
        {
            empowered = false;
            weapon.SetEmpowered(false);
            empowerShot = Timing.RunCoroutine(EmpowerShot(wielder));
        }
    }
    public override void OnUnwield(Player wielder)
    {
        base.OnUnwield(wielder);
        Timing.KillCoroutines(empowerShot);
        empowered = false;
        weapon.SetEmpowered(false);
    }
    IEnumerator<float> EmpowerShot(Player wielder)
    {
        for(int i = 0; i < data.empowerShotCount; i++)
        {
            weapon.Fire();
            base.Fire(wielder);
            yield return Timing.WaitForSeconds(data.empowerShotInterval);
        }
    }
    static ControlElement<bool> canMoveDisable = new(false, 0, true);
    readonly int rollID = Animator.StringToHash("Roll");
    IEnumerator<float> Rolling(Player wielder)
    {
        wielder.movements.canMoveControl.AddControl(canMoveDisable);
        wielder.anim.SetTrigger(rollID);

        float counter = 0.0f;
        float rollDirection = InputManager.Instance.MovementInput();
        while (counter * data.rollSpeed < data.rollDistance)
        {
            wielder.movements.rb.linearVelocityX = rollDirection * data.rollSpeed;
            counter += Time.deltaTime;
            yield return Timing.WaitForOneFrame;
        }
        wielder.movements.canMoveControl.RemoveControl(canMoveDisable);
    }
}