using Unity.Netcode.Components;
using Unity.VisualScripting;
using UnityEngine;

public class NetworkAnimator_Owner : NetworkAnimator
{
    bool initialized = false;
    protected override void Awake()
    {
        Init();
    }
    public void Init()
    {
        if (initialized) return;
        base.Awake();
    }
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
