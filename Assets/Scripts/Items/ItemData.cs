using Unity.Netcode;
using UnityEngine;

public abstract class ItemData : ScriptableObject
{
    [Header("Item")]
    [SerializeField] LangText m_itemName;
    public LangText itemName => m_itemName;
    public abstract Item Create();
    public abstract int maxStack { get; }
}
public abstract class Item
{
    public readonly ItemData data;
    public Item(ItemData data)
    {
        this.data = data;
    }
    //Stacking
    public abstract bool IsStackable(Item other);

    //Wielding
    public virtual void OnWield(Player wielder) { }
    public virtual void OnWieldUpdate(Player wielder) { }
    public virtual void OnUnwield(Player wielder) { }

    //Delete and Copy
    public virtual void Delete() { }
    public virtual Item Copy() => data.Create();
}