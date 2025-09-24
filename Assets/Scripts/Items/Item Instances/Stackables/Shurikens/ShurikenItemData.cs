using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

public abstract class ShurikenItemData : StackableItemData
{
    [Header("Shuriken")]
    [SerializeField] Sprite m_lmbIcon;
    [SerializeField] bool m_auto;
    [SerializeField] float m_damage;
    [SerializeField] CooldownSource m_throwCooldownSource;
    [SerializeField] float m_throwCooldown, m_shurikenLifetime, m_shurikenVelocity, m_spread;
    [SerializeField] Vector2 m_throwOffset = new Vector2(1.0f, 0.0f);
    [SerializeField] float m_throwAngle = 10.0f;
    [SerializeField] Sound m_throwSound;
    public Sprite lmbIcon => m_lmbIcon;
    public bool auto => m_auto;
    public float damage => m_damage;
    public CooldownSource throwCooldownSource => m_throwCooldownSource;
    public float throwCooldown => m_throwCooldown;
    public float shurikenLifetime => m_shurikenLifetime;
    public float shurikenVelocity => m_shurikenVelocity;
    public float spread => m_spread;
    public Vector2 throwOffset => m_throwOffset;
    public float throwAngle => m_throwAngle;
    public Sound throwSound => m_throwSound;
    public abstract Shuriken shuriken { get; }
}
public abstract class ShurikenItem : StackableItem, ILMBUser
{
    new public readonly ShurikenItemData data;
    protected ShurikenItem(ShurikenItemData data) : base(data)
    {
        this.data = data;
    }
    protected float counter = 0.0f;
    public override void OnWield(Player wielder)
    {
        base.OnWield(wielder);
        counter = 0.0f;
    }
    public override void OnWieldUpdate(Player wielder)
    {
        base.OnWieldUpdate(wielder);
        counter += Time.deltaTime;
    }
    protected float throwCounter => data.throwCooldownSource.cooldownLeft;
    protected float throwCooldown => data.throwCooldownSource.cooldown;
    protected virtual bool canThrow => true;
    public Sprite lmbSprite => data.lmbIcon;
    public bool canUseLmb => canThrow && throwCounter <= 0.0f;
    public float lmbCooldownBarFill => throwCounter / throwCooldown;
    readonly int throwID = Animator.StringToHash("Throw");
    public void LMBUse(Player wielder, bool down)
    {
        if (!data.auto && !down || !canUseLmb) return;
        counter = 0.0f;
        wielder.anim.SetTrigger(throwID);
        NetworkAudioManager.Instance.PlaySound(data.throwSound);
        Throw(wielder);
        containedSlot.count--;
    }
    protected virtual void Throw(Player wielder)
    {
        Vector2 pos = (Vector2)wielder.inventory.weaponAnchor.position + (Vector2)wielder.inventory.weaponAnchor.right * data.throwOffset;
        Quaternion rot = wielder.inventory.weaponAnchor.rotation * Quaternion.Euler(0, 0, UnityEngine.Random.Range(-data.spread, data.spread)) * Quaternion.Euler(0, 0, data.throwAngle * wielder.inventory.weaponAnchor.lossyScale.y);
        Shuriken shuriken = MonoBehaviour.Instantiate(data.shuriken, pos, rot);
        shuriken.Set(data.damage, data.shurikenLifetime, data.shurikenVelocity);
        shuriken.NetworkObject.Spawn();
    }
}