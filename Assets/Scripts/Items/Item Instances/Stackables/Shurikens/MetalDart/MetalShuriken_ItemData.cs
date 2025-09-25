using System.Collections.Generic;
using Unity.InferenceEngine;
using Unity.Netcode;
using UnityEngine;
using NetworkAudio;

[CreateAssetMenu(fileName = "Metal Dart", menuName = "Scriptables/Items/Stackables/Darts/Metal Dart")]
public class MetalShuriken_ItemData : ShurikenItemData
{
    [Header("Metal Dart")]
    [SerializeField] MetalShuriken_Shuriken m_shuriken;
    public override Shuriken shuriken => m_shuriken;

    [Header("Triple Throw")]
    [SerializeField] Sprite m_tripleThrowSprite;
    [SerializeField] CooldownSource m_tripleThrowCooldownSource;
    [SerializeField] float m_tripleThrowCooldown;
    [SerializeField] float m_tripleThrowSpread;
    [SerializeField] Sound m_tripleThrowSound;
    public Sprite tripleThrowSprite => m_tripleThrowSprite;
    public CooldownSource tripleThrowCooldownSource => m_tripleThrowCooldownSource;
    public float tripleThrowCooldown => m_tripleThrowCooldown;
    public float tripleThrowSpread => m_tripleThrowSpread;
    public Sound tripleThrowSound => m_tripleThrowSound;
    public override Item Create() => new MetalDart_Item(this);
}
public class MetalDart_Item : ShurikenItem, ISkill1User
{
    new public readonly MetalShuriken_ItemData data;
    public MetalDart_Item(MetalShuriken_ItemData data) : base(data)
    {
        this.data = data;
    }
    float tripleThrowCounter => data.tripleThrowCooldownSource.cooldownLeft;
    float tripleThrowCooldown => data.tripleThrowCooldownSource.cooldown;
    public Sprite skill1Sprite => data.tripleThrowSprite;
    public bool canUseSkill1 => tripleThrowCounter <= 0.0f && containedSlot.count >= 3;
    public float skill1CooldownBarFill => tripleThrowCounter / tripleThrowCooldown;
    public void Skill1Use(Player wielder)
    {
        if (!canUseSkill1) return;
        wielder.cooldowns.AddCooldown(data.tripleThrowCooldownSource, data.tripleThrowCooldown);
        NetworkAudioManager.Instance.PlaySound(data.tripleThrowSound);
        for(int i = 0; i < 3; i++)
        {
            Vector2 pos = (Vector2)wielder.inventory.weaponAnchor.position + (Vector2)wielder.inventory.weaponAnchor.right + data.throwOffset;
            Quaternion rot = wielder.inventory.weaponAnchor.rotation * Quaternion.Euler(0, 0, data.tripleThrowSpread * (i-1));
            Shuriken shuriken = MonoBehaviour.Instantiate(data.shuriken, pos, rot);
            shuriken.Set(data.damage, data.shurikenLifetime, data.shurikenVelocity);
            shuriken.NetworkObject.Spawn();
        }
        containedSlot.count -= 3;
    }
}