using Unity.Netcode;
using UnityEngine;

public class WoodenBow : Bow
{
    readonly int empoweredID = Animator.StringToHash("Empowered");
    public void SetEmpowered(bool empowered) => anim.SetBool(empoweredID, empowered);
}