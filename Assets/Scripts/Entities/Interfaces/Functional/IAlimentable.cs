using Unity.Netcode;
using UnityEngine;

public interface IAlimentable : IRelationInteraction
{
    public void AddAliment(Aliment aliment);
}