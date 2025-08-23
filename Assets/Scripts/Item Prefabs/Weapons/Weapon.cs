using Unity.Netcode;
using Unity.Netcode.Components;
using UnityEngine;

public abstract class Weapon : NetworkBehaviour
{
    public NetworkVariable<bool> enabledState = new NetworkVariable<bool>(true);

    [Header("Weapon")]
    [SerializeField] NetworkAnimator m_anim;
    public NetworkAnimator anim => m_anim;
    protected virtual void Awake()
    {
        enabledState.OnValueChanged += (prev, next) => gameObject.SetActive(next);
    }
}