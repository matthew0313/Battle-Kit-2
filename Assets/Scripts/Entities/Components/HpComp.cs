using Unity.Netcode;
using System;
using UnityEngine;
using System.Net.NetworkInformation;

public abstract class HpComp : NetworkBehaviour, IDamagable
{
    public NetworkBehaviour self => this;

    [SerializeField] float baseHp = 100.0f;
    public readonly NetworkVariable<float> maxHp = new NetworkVariable<float>(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<float> hp = new NetworkVariable<float>(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<bool> dead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

    public Action onHpChange;
    public Action<DamageInfoWrapper> modifyDamageTaken;
    public Action<DamageInfo> onDamage;

    public bool invincible = false;
    public virtual void OnAwake()
    {
        hp.OnValueChanged += (a, b) => onHpChange?.Invoke();
        maxHp.OnValueChanged += (a, b) => onHpChange?.Invoke();
    }
    protected void Init()
    {
        maxHp.Value = baseHp;
        hp.Value = baseHp;
    }

    [Rpc(SendTo.Owner)]
    public void GetDamageRpc(DamageInfo damage)
    {
        if (dead.Value == true || invincible) return;

        DamageInfoWrapper wrap = new() { damage = damage };
        modifyDamageTaken?.Invoke(wrap);
        damage = wrap.damage;

        float amount = Mathf.Min(hp.Value, damage.amount);
        hp.Value -= amount;
        onDamage?.Invoke(damage);

        if (hp.Value <= 0)
        {
            dead.Value = true;
            OnDeath();
        }
    }
    public void GetDamage(DamageInfo damage) => GetDamageRpc(damage);
    protected virtual void OnDeath() { }
    public void Revive()
    {
        if (!IsOwner) return;
        hp.Value = maxHp.Value;
        dead.Value = false;
    }
    public abstract bool IsEnemy(Player other);
    /*public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        DamageInfoWrapper wrapper = new() { damage = damage };
        modifyDamageTaken?.Invoke(wrapper);
        damage = wrapper.damage;

        GetDamageRpc(damage);
        return ProcessDamage(damage);
    }
    protected virtual DamageReceivedInfo ProcessDamage(DamageInfo damage)
    {
        if (dead || invincible) return new() { ignored = true };
        float amount = Mathf.Min(hp, damage.amount);
        hp -= amount;
        DamageReceivedInfo info = new()
        {
            damage = damage,
            receivedAmount = amount,
            fatal = hp <= 0
        };
        onDamageTake?.Invoke(info);
        if (IsOwner)
        {
            if(hp <= 0)
            {
                DeathRpc(LocalDeferMode.SendImmediate);
            }
        }
        return info;
    }*/
}
public class DamageInfoWrapper
{
    public DamageInfo damage;
}