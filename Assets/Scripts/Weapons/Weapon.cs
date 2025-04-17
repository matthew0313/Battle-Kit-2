using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public NetworkVariable<bool> enabledState = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);
    protected virtual void Awake()
    {
        enabledState.OnValueChanged += (prev, next) => gameObject.SetActive(next);
    }
}