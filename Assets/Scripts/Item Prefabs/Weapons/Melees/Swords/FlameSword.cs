using Unity.Netcode;
using UnityEngine;

public class FlameSword : Sword
{
    readonly int flameBlessingID = Animator.StringToHash("FlameBlessing");
    public void FlameBlessing() => anim.SetTrigger(flameBlessingID);
}