using Unity.Netcode;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public NetworkVariable<bool> enabledState = new NetworkVariable<bool>(true);

    [Header("Weapon")]
    [SerializeField] protected Animator anim;
    protected virtual void Awake()
    {
        enabledState.OnValueChanged += (prev, next) => gameObject.SetActive(next);
    }
}