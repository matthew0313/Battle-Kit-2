using Unity.Netcode;
using System;
using UnityEngine;

public class HpComp : NetworkBehaviour, IDamagable
{
    public ulong ownerID => OwnerClientId;
    [SerializeField] float baseHp = 100.0f;
    float m_maxHp;
    public float maxHp
    {
        get => m_maxHp;
        set
        {
            m_maxHp = value;
            onHpChange?.Invoke();
        }
    }
    float m_hp;
    public float hp
    {
        get => m_hp;
        set
        {
            m_hp = value;
            onHpChange?.Invoke();
        }
    }
    public Action onHpChange;

    [Rpc(SendTo.Everyone, AllowTargetOverride = true)]
    public void SetMaxHpRpc(float maxHp, RpcParams rpcParams) => this.maxHp = maxHp;

    [Rpc(SendTo.Everyone, AllowTargetOverride = true)]
    public void SetHpRpc(float hp, RpcParams rpcParams) => this.hp = hp;

    public Action<DamageInfoWrapper> modifyDamageTaken;
    public Action<DamageReceivedInfo> onDamageTake;
    public Action onDeath;
    [Rpc(SendTo.NotMe)]
    public void GetDamageRpc(DamageInfo damage)
    {
        ProcessDamage(damage);
    }
    public DamageReceivedInfo GetDamage(DamageInfo damage)
    {
        GetDamageRpc(damage);
        return ProcessDamage(damage);
    }
    public bool dead { get; private set; } = false;
    public bool invincible = false;
    DamageReceivedInfo ProcessDamage(DamageInfo damage)
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
        if(hp <= 0)
        {
            OnDeath();
        }
        return info;
    }
    protected virtual void OnDeath()
    {
        onDeath?.Invoke();
    }
    private void Awake()
    {
        NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.Singleton.OnClientConnectedCallback -= OnClientConnected;
    }
    void OnClientConnected(ulong clientId)
    {
        SetMaxHpRpc(maxHp, RpcTarget.Single(clientId, RpcTargetUse.Temp));
        SetHpRpc(maxHp, RpcTarget.Single(clientId, RpcTargetUse.Temp));
    }
}
public class DamageInfoWrapper
{
    public DamageInfo damage;
}