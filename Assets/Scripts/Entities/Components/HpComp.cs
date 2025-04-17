using Unity.Netcode;
using System;
using UnityEngine;

public class HpComp : NetworkBehaviour, IDamagable
{
    public ulong ownerID => NetworkObject.OwnerClientId;

    [SerializeField] float baseHp = 100.0f;
    public readonly NetworkVariable<float> maxHp = new NetworkVariable<float>(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<float> hp = new NetworkVariable<float>(100.0f, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    public readonly NetworkVariable<bool> dead = new NetworkVariable<bool>(false, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    
    public Action<DamageInfoWrapper> modifyDamageTaken;
    public Action<DamageInfo> onDamage;
    public Action onDeath;

    public bool invincible = false;
    private void Start()
    {
        maxHp.Value = baseHp;
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
        if(hp.Value <= 0)
        {
            dead.Value = false;
            onDeath?.Invoke();
        }
    }
    public void GetDamage(DamageInfo damage) => GetDamageRpc(damage);
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