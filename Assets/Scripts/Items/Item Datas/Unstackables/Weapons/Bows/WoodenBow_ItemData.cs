using System;
using System.Collections.Generic;
using MEC;
using UnityEngine;

[CreateAssetMenu(fileName = "Wooden Bow", menuName = "Scriptables/Items/Unstackables/Weapons/Bows/Wooden Bow")]
public class WoodenBow_ItemData : BowItemData
{
    [Header("Wooden Bow")]
    [SerializeField] WoodenBow m_weaponPrefab;

    [Header("Empower")]
    [SerializeField] Sprite m_empowerIcon;
    [SerializeField] float m_empowerCooldown = 10.0f;
    [SerializeField] int m_empowerShotCount = 3;
    [SerializeField] float m_empowerShotInterval = 0.1f;
    public Sprite empowerIcon => m_empowerIcon;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public float empowerCooldown => m_empowerCooldown;
    public int empowerShotCount => m_empowerShotCount;
    public float empowerShotInterval => m_empowerShotInterval;
    public override Item Create() => new WoodenBow_Item(this);
}
public class WoodenBow_Item : BowItem, ISkill1User
{
    new public readonly WoodenBow_ItemData data;
    new protected WoodenBow bow => base.bow as WoodenBow;
    public WoodenBow_Item(WoodenBow_ItemData data) : base(data)
    {
        this.data = data;
    }
    float empowerCounter = 0.0f;
    bool empowered
    {
        get => bow.empowered.Value;
        set => bow.empowered.Value = value;
    }
    public Sprite skill1Sprite => data.empowerIcon;
    public bool canUseSkill1 => !empowered && empowerCounter <= 0.0f;
    public float skill1CooldownBarFill => empowerCounter / data.empowerCooldown;
    public void Skill1Use()
    {
        Debug.Log("Skill1Used");
        if (!canUseSkill1) return;
        empowerCounter = data.empowerCooldown;
        empowered = true;
    }
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        if(!empowered) empowerCounter = Mathf.Max(0.0f, empowerCounter - Time.deltaTime);
    }
    protected override void Fire(Player wielder)
    {
        if(!empowered) base.Fire(wielder);
        else
        {
            empowered = false;
            Timing.RunCoroutine(EmpowerShot(wielder, bow.firePoint.position, bow.firePoint.rotation));
        }
    }
    IEnumerator<float> EmpowerShot(Player wielder, Vector2 pos, Quaternion rot)
    {
        for(int i = 0; i < data.empowerShotCount; i++)
        {
            Bullet bul = MonoBehaviour.Instantiate(data.bullet, pos, rot);
            bul.Set(GetOnHit(wielder), data.arrowSpeed, data.arrowRange);
            bul.NetworkObject.Spawn();
            yield return Timing.WaitForSeconds(data.empowerShotInterval);
        }
    }
}