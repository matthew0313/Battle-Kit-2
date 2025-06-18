using Unity.Netcode;
using UnityEngine;

public class WoodenBow : Bow
{
    public NetworkVariable<bool> empowered = new NetworkVariable<bool>();
    protected override void Awake()
    {
        base.Awake();
        empowered.OnValueChanged += OnEmpoweredStateChange;
    }
    readonly int empoweredID = Animator.StringToHash("Empowered");
    void OnEmpoweredStateChange(bool prev, bool empowered) => anim.SetBool(empoweredID, empowered);
}