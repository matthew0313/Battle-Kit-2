using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

[CreateAssetMenu(fileName = "Metal Dart", menuName = "Scriptables/Items/Stackables/Darts/Metal Dart")]
public class MetalDart_ItemData : DartItemData
{
    [Header("Metal Dart")]
    [SerializeField] MetalDart_Dart m_dart;
    public override Dart dart => m_dart;
    public override Item Create() => new MetalDart_Item(this);
}
public class MetalDart_Item : DartItem
{
    new public readonly MetalDart_ItemData data;
    public MetalDart_Item(MetalDart_ItemData data) : base(data)
    {
        this.data = data;
    }
}