using System;
using System.Collections.Generic;
using MEC;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "FlameSword", menuName = "Scriptables/Items/Unstackables/Weapons/Melees/Swords/FlameSword")]
public class FlameSword_ItemData : SwordItemData
{
    [Header("FlameSword")]
    [SerializeField] FlameSword m_weaponPrefab;
    [SerializeField] float m_burnDuration, m_burnDamage;
    public override Weapon weaponPrefab => m_weaponPrefab;
    public float burnDuration => m_burnDuration;
    public float burnDamage => m_burnDamage;
    public override Item Create() => new FlameSword_Item(this);
}
public class FlameSword_Item : SwordItem
{
    new public readonly FlameSword_ItemData data;
    new protected FlameSword weapon => base.weapon as FlameSword;

    public FlameSword_Item(FlameSword_ItemData data) : base(data)
    {
        this.data = data;
    }
    protected override void OnHit(GameObject hit)
    {
        base.OnHit(hit);
        if(hit.TryGetComponent(out HpComp hpComp))
        {
            if(wielder.GetRelation(hpComp.owner) == Relation.Enemy)
            {
                hpComp.AddAliment(new Burning(data.burnDuration, data.burnDamage));
            }
        }
    }
}