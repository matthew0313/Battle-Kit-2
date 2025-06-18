using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class DartItemData : StackableItemData
{
    [Header("Dart")]
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage, m_fireRate, m_bulletSpeed, m_bulletRange, m_spread;
    public bool auto => m_auto;
    public float damage => m_damage;
    public float fireRate => m_fireRate;
    public float bulletSpeed => m_bulletSpeed;
    public float bulletRange => m_bulletRange;
    public float spread => m_spread;
    protected abstract Bullet bullet { get; }
}
public abstract class DartItem : StackableItem, ILMBUser
{
    new public readonly DartItemData data;
    protected DartItem(DartItemData data) : base(data)
    {
        this.data = data;
    }
    public void LMBUse(Player wielder, bool down)
    {
        throw new System.NotImplementedException();
    }
}