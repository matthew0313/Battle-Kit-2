using Unity.Netcode;
using System;
using UnityEngine;
using System.Net.NetworkInformation;
using System.Collections.Generic;
using Unity.Collections;

public abstract class HpComp : NetworkBehaviour, IDamagable, IAlimentable
{
    public abstract Entity owner { get; }

    [SerializeField] float baseHp = 100.0f;
    public readonly NetworkVariable<float> maxHp = new NetworkVariable<float>(100.0f);
    public readonly NetworkVariable<float> hp = new NetworkVariable<float>(100.0f);
    public readonly NetworkVariable<bool> dead = new NetworkVariable<bool>(false);

    public event Action onHpChange;
    public event Action<DamageInfoWrapper> modifyDamageTaken;
    public event Action<DamageReceivedInfo> onDamage;

    public bool invincible = false;
    public readonly Dictionary<ulong, Aliment> aliments = new();
    public event Action onAlimentsUpdate;


    [Rpc(SendTo.Owner)]
    public void ResetHpRpc()
    {
        maxHp.Value = baseHp;
        hp.Value = baseHp;
    }
    protected virtual void Awake()
    {
        hp.OnValueChanged += (a, b) => onHpChange?.Invoke();
        maxHp.OnValueChanged += (a, b) => onHpChange?.Invoke();
        NetworkManager.OnConnectionEvent += OnConnectionEvent;

    }
    protected virtual void Start()
    {
        if (IsOwner)
        {
            maxHp.Value = baseHp;
            hp.Value = baseHp;
        }
    }
    readonly List<ulong> alimentRemoveQueue = new();
    protected virtual void Update()
    {
        if(alimentRemoveQueue.Count > 0)
        {
            foreach (var i in alimentRemoveQueue)
            {
                aliments[i].OnAlimentRemove(this);
                aliments.Remove(i);
            }
            alimentRemoveQueue.Clear();
            onAlimentsUpdate?.Invoke();
        }
        foreach(var i in aliments)
        {
            i.Value.OnAlimentUpdate(this);
        }
    }
    public override void OnDestroy()
    {
        base.OnDestroy();
        NetworkManager.OnConnectionEvent -= OnConnectionEvent;
    }
    void OnConnectionEvent(NetworkManager manager, ConnectionEventData data)
    {
        if (!IsOwner) return;
        if(data.EventType == ConnectionEvent.ClientConnected)
        {
            foreach(var i in aliments)
            {
                AddAlimentInstanceRpc(i.Key, i.Value.ToInstanceData(), RpcTarget.Single(data.ClientId, RpcTargetUse.Temp));
            }
        }
    }
    public void GetDamage(DamageInfo damage) => GetDamageRpc(damage);

    [Rpc(SendTo.Authority)]
    public void GetDamageRpc(DamageInfo damage) => OnDamageRpc(ProcessDamage(damage));
    protected virtual DamageReceivedInfo ProcessDamage(DamageInfo damage)
    {
        DamageReceivedInfo receivedInfo = new() { damage = damage };
        if (dead.Value == true || invincible)
        {
            receivedInfo.isNullified = true;
            return receivedInfo;
        }

        DamageInfoWrapper wrap = new() { damage = damage };
        modifyDamageTaken?.Invoke(wrap);
        damage = wrap.damage;

        float amount = Mathf.Min(hp.Value, damage.amount);
        hp.Value -= amount;

        if (hp.Value <= 0)
        {
            dead.Value = true;
            OnDeath();
        }
        return receivedInfo;
    }
    [Rpc(SendTo.Everyone)]
    public void OnDamageRpc(DamageReceivedInfo info) => onDamage?.Invoke(info);
    protected virtual void OnDeath() { }
    public void Revive()
    {
        if (!IsOwner) return;
        dead.Value = false;
        OnRevive();
    }
    protected virtual void OnRevive() { hp.Value = maxHp.Value; }

    [Rpc(SendTo.Authority)]
    public void AddAlimentRpc(AlimentInstanceData info)
    {
        Aliment tmp = info.ToAliment();
        tmp.AddTo(this);
    }
    public void AddAliment(Aliment aliment)
    {
        if (IsOwner)
        {
            aliment.AddTo(this);
        }
        else
        {
            AddAlimentRpc(aliment.ToInstanceData());
        }
    }
    [Rpc(SendTo.Everyone, AllowTargetOverride = true)]
    public void AddAlimentInstanceRpc(ulong id, AlimentInstanceData info, RpcParams rpcParams)
    {
        if (aliments.ContainsKey(id))
        {
            aliments[id].OnAlimentRemove(this);
            aliments.Remove(id);
        }

        Aliment tmp = info.ToAliment();
        tmp.id = id;
        tmp.OnAlimentAdd(this);
        aliments[id] = tmp;
        onAlimentsUpdate?.Invoke();
    }
    [Rpc(SendTo.Everyone)]
    public void DeleteAlimentInstanceRpc(ulong id)
    {
        if (!aliments.ContainsKey(id) || alimentRemoveQueue.Contains(id)) return;
        alimentRemoveQueue.Add(id);
    }
    [Rpc(SendTo.NotMe)]
    public void SetAlimentDurationRpc(ulong id, float duration)
    {
        if (!aliments.ContainsKey(id) || alimentRemoveQueue.Contains(id)) return;
        aliments[id].SetDuration(duration);
    }

    public IEnumerable<Aliment> GetAliments()
    {
        foreach (var i in aliments) yield return i.Value;
    }
}
public class DamageInfoWrapper
{
    public DamageInfo damage;
}
public struct DamageReceivedInfo : INetworkSerializable
{
    public DamageInfo damage;

    public bool isNullified;

    public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
    {
        serializer.SerializeNetworkSerializable(ref damage);
        serializer.SerializeValue(ref isNullified);
    }
}